using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    private GameObject RankObj,UsernameObj,NGameObj,AcceptanceObj;

    private GameObject RankTemp,UserTemp,NGameTemp,AccTemp;

    void Start()
    {
        this.RankObj=lobbyController.getChildByName(this.gameObject,"RANK");
        this.UsernameObj=lobbyController.getChildByName(this.gameObject,"USERNAME");
        this.NGameObj=lobbyController.getChildByName(this.gameObject,"NGAME");
        this.AcceptanceObj=lobbyController.getChildByName(this.gameObject,"ACC");

        this.RankTemp=lobbyController.getChildByName(this.gameObject,"RANK3");
        this.UserTemp=lobbyController.getChildByName(this.gameObject,"USERNAME3");
        this.NGameTemp=lobbyController.getChildByName(this.gameObject,"NGAME3");
        this.AccTemp=lobbyController.getChildByName(this.gameObject,"ACC3");

        // lobbyController
    }

    public void addRow(string rank,string username,string games,string acceptance){
        GameObject newRank = Instantiate(RankTemp,Vector3.zero,Quaternion.identity);
        newRank.GetComponent<TextMeshProUGUI>().text=rank;
        newRank.transform.SetParent(RankObj.transform,false);


        GameObject newUser = Instantiate(UserTemp,Vector3.zero,Quaternion.identity);
        newUser.GetComponent<TextMeshProUGUI>().text=username;
        newUser.transform.SetParent(UsernameObj.transform,false);


        GameObject newGame = Instantiate(NGameTemp,Vector3.zero,Quaternion.identity);
        newGame.GetComponent<TextMeshProUGUI>().text=games;
        newGame.transform.SetParent(NGameObj.transform,false);

        GameObject newAccep = Instantiate(AccTemp,Vector3.zero,Quaternion.identity);
        newAccep.GetComponent<TextMeshProUGUI>().text=acceptance;
        newAccep.transform.SetParent(AcceptanceObj.transform,false);
    }

    public void hideLoading(){
                GameObject toBeDelete = lobbyController.getChildByName(this.gameObject,"Loading");
                toBeDelete.SetActive(false);

    }

}
