using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTimed : MonoBehaviour {

    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject gate;
    [SerializeField] private AudioClip openGateClip;
    [SerializeField] private AudioClip failGateClip;
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip timerTickClip;
    [SerializeField] private float timerDuration = 10f;
    [SerializeField] private float initialTickRate = 0.75f;
    [SerializeField] private float finalTickRate = 0.05f;

    private float pressedButtonsCount;
    private float timer;
    private Coroutine timerWaitRoutine;
    private Coroutine timerTickRoutine;

    private const float INITIAL_POS_Y = 0.2f;
    private const float PRESSED_POS_Y = 0.05f;
    private Color COLOR_LOCKED = Color.red;
    private Color COLOR_UNLOCKED = Color.green;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        foreach (GameObject button in this.buttons)
        {
            OnTriggerEnterListener listener = button.AddComponent<OnTriggerEnterListener>();
            listener.onTriggerEnterDelegate = ButtonPressed;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void ResetGate()
    {
        this.gate.SetActive(true);

        this.pressedButtonsCount = 0;
        foreach (GameObject button in this.buttons)
        {
            button.transform.localPosition = new Vector3(0f, GateTimed.INITIAL_POS_Y, 0f);
            button.GetComponent<MeshRenderer>().material.color = this.COLOR_LOCKED;
        }

        StopAllRoutines();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void ButtonPressed(GameObject button)
    {
        if (button.transform.localPosition.y != GateTimed.PRESSED_POS_Y)
        {
            this.pressedButtonsCount++;
            button.transform.localPosition = new Vector3(0f, GateTimed.PRESSED_POS_Y, 0f);
            SoundManager.instance.PlaySound(this.buttonPressClip);

            if (this.pressedButtonsCount == 1)
            {
                this.timerWaitRoutine = StartCoroutine(WaitForTimer());
                this.timerTickRoutine = StartCoroutine(TimerTick());
            }

            if (this.pressedButtonsCount == this.buttons.Length)
            {
                foreach (GameObject buttonObject in this.buttons)
                {
                    buttonObject.GetComponent<MeshRenderer>().material.color = this.COLOR_UNLOCKED;
                }

                this.gate.SetActive(false);
                SoundManager.instance.PlaySound(this.openGateClip);
                StopAllRoutines();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void StopAllRoutines()
    {
        if (this.timerWaitRoutine != null)
        {
            StopCoroutine(this.timerWaitRoutine);
            this.timerWaitRoutine = null;
        }

        if (this.timerTickRoutine != null)
        {
            StopCoroutine(this.timerTickRoutine);
            this.timerTickRoutine = null;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator WaitForTimer()
    {
        yield return new WaitForSeconds(this.timerDuration);
        SoundManager.instance.PlaySound(this.failGateClip);
        ResetGate();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator TimerTick()
    {
        float timer = 0f;
        while (true)
        {
            float tickTime = Mathf.Lerp(this.initialTickRate, this.finalTickRate, timer / this.timerDuration);
            timer += tickTime;

            yield return new WaitForSeconds(tickTime);

            SoundManager.instance.PlaySound(this.timerTickClip);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnReset(bool isFirstSpawn)
    {
        ResetGate();
    }
}
