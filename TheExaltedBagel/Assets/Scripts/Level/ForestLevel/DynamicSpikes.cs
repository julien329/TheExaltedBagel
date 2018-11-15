using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpikes : MonoBehaviour {

    [SerializeField] private GameObject dynamicSpikes;
    [SerializeField] private AudioClip audioSpikeFall;
    [SerializeField] private int numberBlocToGo;
    [SerializeField] private bool directionXaxis = false;
    [SerializeField] private float speedUnitPerSecond = 2.0f;

    private AudioSource m_AudioSource;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        //Fetch the AudioSource component of the GameObject (make sure there is one in the Inspector)
        m_AudioSource = GetComponent<AudioSource>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && LevelManager.instance.IsDynamicSpikesReset)
        {
            m_AudioSource.Play();
            SoundManager.instance.PlaySound(audioSpikeFall);

            this.dynamicSpikes.SetActive(true);

            StartCoroutine(MoveOverSpeed());         
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player") && LevelManager.instance.IsDynamicSpikesReset)
        {
            LevelManager.instance.IsDynamicSpikesReset = false;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public IEnumerator MoveOverSpeed()
    {
        Vector3 startPosition = dynamicSpikes.transform.position;
        Vector3 endPosition = (directionXaxis) ?
            new Vector3(startPosition.x + (float)this.numberBlocToGo, startPosition.y, startPosition.z) :
            new Vector3(startPosition.x, startPosition.y + (float)this.numberBlocToGo, startPosition.z);

        // speed should be 1 unit per second
        while (this.dynamicSpikes.transform.position != endPosition)
        {
            if (LevelManager.instance.IsPlayerDead())
            {
                break;
            }

            this.dynamicSpikes.transform.position = Vector3.MoveTowards(this.dynamicSpikes.transform.position, endPosition, this.speedUnitPerSecond * Time.deltaTime);
            yield return null;
        }

        m_AudioSource.Stop();
        SoundManager.instance.PlaySound(this.audioSpikeFall);
    }

}
