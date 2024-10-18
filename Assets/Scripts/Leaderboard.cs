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
        GameObject ListObj = lobbyController.getChildByName(lobbyController.getChildByName(this.gameObject,"Scroll View"),"List");
        this.RankObj=lobbyController.getChildByName(ListObj,"RANK");
        this.UsernameObj=lobbyController.getChildByName(ListObj,"USERNAME");
        this.NGameObj=lobbyController.getChildByName(ListObj,"NGAME");
        this.AcceptanceObj=lobbyController.getChildByName(ListObj,"ACC");

        this.RankTemp=lobbyController.getChildByName(this.gameObject,"RANK3");
        this.UserTemp=lobbyController.getChildByName(this.gameObject,"USERNAME3");
        this.NGameTemp=lobbyController.getChildByName(this.gameObject,"NGAME3");
        this.AccTemp=lobbyController.getChildByName(this.gameObject,"ACC3");

        // lobbyController
    }

   public void addRow(string rank, string username, string games, string acceptance)
{
    GameObject ScrollObj = lobbyController.getChildByName(this.gameObject, "Scroll View");
    RectTransform scrollRectTransform = ScrollObj.GetComponent<RectTransform>(); 

    GameObject newRank = Instantiate(RankTemp, Vector3.zero, Quaternion.identity);
    newRank.GetComponent<TextMeshProUGUI>().text = rank;
    newRank.transform.SetParent(RankObj.transform, false);

    GameObject newUser = Instantiate(UserTemp, Vector3.zero, Quaternion.identity);
    newUser.GetComponent<TextMeshProUGUI>().text = username;
    newUser.transform.SetParent(UsernameObj.transform, false);

    GameObject newGame = Instantiate(NGameTemp, Vector3.zero, Quaternion.identity);
    newGame.GetComponent<TextMeshProUGUI>().text = games;
    newGame.transform.SetParent(NGameObj.transform, false);

    GameObject newAccep = Instantiate(AccTemp, Vector3.zero, Quaternion.identity);
    newAccep.GetComponent<TextMeshProUGUI>().text = acceptance;
    newAccep.transform.SetParent(AcceptanceObj.transform, false);

    float spacing = 1f;
     float panelHeight = scrollRectTransform.GetComponent<RectTransform>().rect.height;

    // scrollRectTransform.sizeDelta = new Vector2(
    //     scrollRectTransform.sizeDelta.x, 
    //     scrollRectTransform.sizeDelta.y + spacing
    // );

    scrollRectTransform.sizeDelta = new Vector2(scrollRectTransform.sizeDelta.x,
            scrollRectTransform.sizeDelta.y + 0 + spacing);
}


    public void hideLoading(){
                GameObject toBeDelete = lobbyController.getChildByName(this.gameObject,"Loading");
                toBeDelete.SetActive(false);

    }

}
