using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip footStepLeft;
    public AudioClip footStepRight;

    public AudioClip getHit;
    public AudioClip die;

    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public AudioClip attack4;
    public AudioClip attackHit;

    public AudioClip defendEffect;
    public AudioClip swordEffect;

    public PlayerController playerController;

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        playerController = this.GetComponent<PlayerController>();
    }

    void SoundFootstepLeft()
    {
        if (footStepLeft)
            audioSource.PlayOneShot(footStepLeft, 0.3f);
        Debug.Log("SoundFootstepLeft");
    }

    void SoundFootstepRight()
    {
        if (footStepRight)
            audioSource.PlayOneShot(footStepRight, 0.3f);
        Debug.Log("SoundFootstepRight");
    }

    void SoundAttack1()
    {
        if (attack1)
            audioSource.PlayOneShot(attack1, 0.7f);
        SoundAttackHit();
        Debug.Log("SoundAttack1");
    }

    void SoundAttack2()
    {
        if (attack2)
            audioSource.PlayOneShot(attack2, 0.7f);
        SoundAttackHit();
        Debug.Log("SoundAttack2");
    }

    void SoundAttack3()
    {
        if (attack3)
            audioSource.PlayOneShot(attack3, 0.7f);
        SoundAttackHit();
        Debug.Log("SoundAttack3");
    }

    void SoundAttack4()
    {
        if (attack4)
            audioSource.PlayOneShot(attack4, 0.7f);
        SoundAttackHit();
        Debug.Log("PlayOneShot");
    }

    void SoundAttackHit()
    {
        if (attackHit && playerController.touchedEnemy)
        {
            audioSource.PlayOneShot(attackHit, 0.5f);
            playerController.touchedEnemy = false;
        }
    }

    void SoundAuraShield()
    {
        if (defendEffect)
            audioSource.PlayOneShot(defendEffect, 1f);
        Debug.Log("SoundAuraShield");
    }
    void SoundAuraSword()
    {
        if (swordEffect)
            audioSource.PlayOneShot(swordEffect, 1f);
        Debug.Log("SoundAuraSword");
    }

    void SoundGetHit()
    {
        if (getHit)
            audioSource.PlayOneShot(getHit, 1f);
        Debug.Log("SoundgetHit");
    }

    void SoundDie()
    {
        if (getHit)
            audioSource.PlayOneShot(die, 1f);
        Debug.Log("SoundDie");
    }
}
