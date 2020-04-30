using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEnemy : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip attack;
    public AudioClip getHit;
    public AudioClip die;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    void SoundAttack1()
    {
        if (attack)
            audioSource.PlayOneShot(attack, 1f);
        Debug.Log("SoundAttack");
    }

    void SoundGetHit()
    {
        if (getHit)
            audioSource.PlayOneShot(getHit, 1f);
        Debug.Log("SoundgetHit");
    }

    void SoundDie()
    {
        if (die)
            audioSource.PlayOneShot(die, 1f);
        Debug.Log("SoundDie");
    }
}
