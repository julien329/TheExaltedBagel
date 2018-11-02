﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] uint channelCount = 16;

    private AudioSource[] audioSources;
    private Queue<uint> availableChannelIndexes;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        SoundManager.instance = this;

        this.availableChannelIndexes = new Queue<uint>();
        this.audioSources = new AudioSource[this.channelCount];
        for (uint i = 0; i < this.channelCount; ++i)
        {
            this.availableChannelIndexes.Enqueue(i);
            this.audioSources[i] = this.gameObject.AddComponent<AudioSource>();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlaySound(AudioClip audioClip)
    {
        if (this.availableChannelIndexes.Count > 0)
        {
            uint index = this.availableChannelIndexes.Dequeue();

            AudioSource audioSource = this.audioSources[index];
            audioSource.PlayOneShot(audioClip);
            StartCoroutine(WaitForEndOfClip(index, audioClip.length));
        }
        else
        {
            print("Warning: All sound channels are used, cannot play the requested sound");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator WaitForEndOfClip(uint index, float duration)
    {
        yield return new WaitForSeconds(duration);
        this.availableChannelIndexes.Enqueue(index);
    }
}
