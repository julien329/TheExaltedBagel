using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private float ambiantVolumeScale = 0.5f;
    [SerializeField] private AudioClip ambiantClip;
    [SerializeField] private uint channelCount = 16;

    private AudioSource ambiantSource;
    private AudioSource[] audioSources;
    private Queue<uint> availableChannelIndexes;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        SoundManager.instance = this;

        this.ambiantSource = this.gameObject.AddComponent<AudioSource>();
        if (this.ambiantClip != null)
        {
            this.ambiantSource.clip = this.ambiantClip;
            this.ambiantSource.loop = true;
            this.ambiantSource.volume = this.ambiantVolumeScale;
            this.ambiantSource.Play();
        }

        this.availableChannelIndexes = new Queue<uint>();
        this.audioSources = new AudioSource[this.channelCount];
        for (uint i = 0; i < this.channelCount; ++i)
        {
            this.availableChannelIndexes.Enqueue(i);
            this.audioSources[i] = this.gameObject.AddComponent<AudioSource>();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlaySound(AudioClip audioClip, float volumeScale = 1.0f)
    {
        if (this.availableChannelIndexes.Count > 0)
        {
            if (audioClip != null)
            {
                uint index = this.availableChannelIndexes.Dequeue();

                AudioSource audioSource = this.audioSources[index];
                audioSource.PlayOneShot(audioClip, volumeScale);
                StartCoroutine(WaitForEndOfClip(index, audioClip.length));
            }
            else
            {
                print("Warning: The requested AudioClip is missing.");
            }
        }
        else
        {
            print("Warning: All sound channels are used, cannot play the requested sound.");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void StopAllSounds()
    {
        foreach (AudioSource audioSource in this.audioSources)
        {
            audioSource.Stop();
        }

        this.ambiantSource.Stop();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator WaitForEndOfClip(uint index, float duration)
    {
        yield return new WaitForSeconds(duration);
        this.availableChannelIndexes.Enqueue(index);
    }
}
