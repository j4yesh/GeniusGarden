using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audiosource; 
    public AudioClip clickSound, correctAnswerSound, wrongAnswerSound, newPlayerEnterSound, resultSound;

    void Start()
    {
        audiosource = this.gameObject.GetComponent<AudioSource>();
    }

    public void click()
    {
        PlaySound(clickSound);
    }

    public void correctAnswer()
    {
        PlaySound(correctAnswerSound);
    }

    public void wrongAnswer()
    {
        PlaySound(wrongAnswerSound);
    }

    public void newPlayerEnter()
    {
        PlaySound(newPlayerEnterSound);
    }

    public void result()
    {
      
        audiosource.PlayOneShot(resultSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != resultSound)  
        {
            audiosource.clip = clip;
            audiosource.Play();
        }
    }
}
