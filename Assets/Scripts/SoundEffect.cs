using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audiosource; 
    public AudioClip clickSound, correctAnswerSound, wrongAnswerSound, newPlayerEnterSound, resultSound,
                     tryAgainSound;

    public GameObject BGM;

    private bool allowed ;

    void Start()
    {
        audiosource = this.gameObject.GetComponent<AudioSource>();
    }

    public void click()
    {   
        if(!allowed)return;
        PlaySound(clickSound);
    }

    public void correctAnswer()
    {
        if(!allowed)return;
        PlaySound(correctAnswerSound);
    }

   public void wrongAnswer()
    {   
        if(!allowed)return;

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
        if(!allowed)return;

        PlaySound(newPlayerEnterSound);
    }

    public void result()
    {
        if(!allowed)return;
        
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

    public void enableBGM(bool decide=true){
        this.BGM.GetComponent<AudioSource>().mute = decide;
    }

    public void disableBGM(bool decide=false){
        this.BGM.GetComponent<AudioSource>().mute = decide;
    }

    public void enableSFX(bool decide=true){
        this.allowed=decide;
    }
    public void disableSFX(bool decide=false){
        this.allowed=decide;
    }
    public void toggleBGM(){
        this.BGM.GetComponent<AudioSource>().mute = (this.BGM.GetComponent<AudioSource>().mute)?false:true;
        SaveData saveData = SaveManager.LoadGameState();
        saveData.bgmAllowed = this.BGM.GetComponent<AudioSource>().mute;
        SaveManager.SaveGameState(saveData);
    }
    public void toggleSFX(){
        allowed= (allowed)?false:true;
        SaveData saveData = SaveManager.LoadGameState();
        saveData.sfxAllowed = this.BGM.GetComponent<AudioSource>().mute;
        SaveManager.SaveGameState(saveData);
    }
}
