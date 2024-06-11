using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip powerUp;
    public AudioClip playerHit;
    public AudioClip enemyHit;
    public AudioClip playerFire;
    public AudioClip enemyFire;
    public AudioClip gameOver;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopBackground()
    {
        musicSource.Stop();
    }
}
