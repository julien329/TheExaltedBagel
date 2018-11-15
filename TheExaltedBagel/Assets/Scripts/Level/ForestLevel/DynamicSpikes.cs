using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpikes : MonoBehaviour {

    [SerializeField] private GameObject dynamicSpikes;
    [SerializeField] private AudioClip audioSpikeFall;
    [SerializeField] private int numberBlocToGo;
    [SerializeField] private bool directionXaxis = false;
    [SerializeField] private float speedUnitPerSecond = 2.0f;

    private bool isActive = false;
    private AudioSource m_AudioSource;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        //Fetch the AudioSource component of the GameObject (make sure there is one in the Inspector)
        m_AudioSource = GetComponent<AudioSource>();
        //Stop the Audio playing
        m_AudioSource.Stop();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.PlaySound(audioSpikeFall);
            this.dynamicSpikes.SetActive(true);
            Vector3 startPosition = dynamicSpikes.transform.position;
            Vector3 endPosition;
            if (directionXaxis)
            {
                endPosition = new Vector3(startPosition.x + (float)this.numberBlocToGo, startPosition.y, startPosition.z);
            }
            else
            {
                endPosition = new Vector3(startPosition.x, startPosition.y + (float)this.numberBlocToGo, startPosition.z);
            }
            StartCoroutine(MoveOverSpeed(endPosition, speedUnitPerSecond));
            // Sound
            m_AudioSource.Play();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public IEnumerator MoveOverSpeed(Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (this.dynamicSpikes.transform.position != end)
        {
            if(LevelManager.instance.IsPlayerDead())
            {
                break;
            }
            this.dynamicSpikes.transform.position = Vector3.MoveTowards(this.dynamicSpikes.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        m_AudioSource.Stop();
        SoundManager.instance.PlaySound(audioSpikeFall);
    }

}
