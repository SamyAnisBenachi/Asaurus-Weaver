using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip footStepLeft;
    public AudioClip footStepRight;

    public AudioSource audioSource;

    void Start()
    {

    }

    void Update()
    {

    }

    void SoundFootstepLeft()
    {
        audioSource.PlayOneShot(footStepLeft, 0.1f);
        Debug.Log("test L ");
    }

    void SoundFootstepRight()
    {
        audioSource.PlayOneShot(footStepRight, 0.1f);
        Debug.Log("test R");
    }
}
