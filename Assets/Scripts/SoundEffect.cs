using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audiosource;
    public AudioClip clickSound, correctAnswerSound, wrongAnswerSound, newPlayerEnterSound, resultSound, tryAgainSound;
    public GameObject BGM;

    private bool allowed;

    public GameObject bgmToggle1, sfxToggle1;

    void Start()
    {
        audiosource = this.gameObject.GetComponent<AudioSource>();

        SaveData saveData = SaveManager.LoadGameState();
        setBGM(saveData.bgmAllowed);
        setSFX(saveData.sfxAllowed);

        bgmToggle1.GetComponent<Toggle>().isOn = !BGM.GetComponent<AudioSource>().mute;
        sfxToggle1.GetComponent<Toggle>().isOn = allowed;
    }

    public void click()
    {
        if (!allowed) return;
        PlaySound(clickSound);
    }

    public void correctAnswer()
    {
        if (!allowed) return;
        PlaySound(correctAnswerSound);
    }

    public void wrongAnswer()
    {
        if (!allowed) return;

        int ri = Random.Range(0, 5);

        if (ri >= 1)
        {
            PlaySound(wrongAnswerSound);
            DOVirtual.DelayedCall(wrongAnswerSound.length, () => {
                PlaySound(tryAgainSound);
            });
        }
        else
        {
            PlaySound(wrongAnswerSound);
        }
    }

    public void newPlayerEnter()
    {
        if (!allowed) return;
        PlaySound(newPlayerEnterSound);
    }

    public void result()
    {
        if (!allowed) return;
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

    public void setBGM(bool decide)
    {
        this.BGM.GetComponent<AudioSource>().mute = !decide; 
    }

    public void setSFX(bool decide)
    {
        this.allowed = decide;
    }

    public void toggleBGM()
    {
        bool isOn = bgmToggle1.GetComponent<Toggle>().isOn;
        setBGM(isOn);

        SaveData saveData = SaveManager.LoadGameState();
        saveData.bgmAllowed = isOn;
        SaveManager.SaveGameState(saveData);
    }

    public void toggleSFX()
    {
        bool isOn = sfxToggle1.GetComponent<Toggle>().isOn;
        setSFX(isOn);

        SaveData saveData = SaveManager.LoadGameState();
        saveData.sfxAllowed = isOn;
        SaveManager.SaveGameState(saveData);
    }
}
