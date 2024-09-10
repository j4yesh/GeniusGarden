using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lobbyController : MonoBehaviour
{
    private GameObject canvas1,HostCanvas,JoinCanvas;
    public string username,joinRoomId;
    private TMP_InputField  selfUsername;

    private TMP_InputField  TMPjoinRoomId;
    public FloatingJoystick __joystick;


    private string filePath;
    
    public GameObject Gameplay;
    public GameObject PlayerEntryUI ;

    private GameObject playerTable;

    private string roomId = "";
    void Start()
    {   
        canvas1 = transform.Find("1")?.gameObject;
        HostCanvas = transform.Find("HostCanvas")?.gameObject;
        JoinCanvas = transform.Find("JoinCanvas")?.gameObject;
        selfUsername = canvas1.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField >();
        TMPjoinRoomId = JoinCanvas.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField>();
        playerTable=HostCanvas.transform.Find("BACK")?.gameObject;
        canvas1.SetActive(true);
        HostCanvas.SetActive(false);
        __joystick.SetActive(false);
        Debug.Log(env.API_URL);
    }

    void Update()
    {
    }

    public void hostGame(){
        roomId = GenerateRandomEndpoint();
        string Endpoint = env.API_URL+'/'+roomId;
        Debug.Log(Endpoint);
        canvas1.SetActive(false);
        HostCanvas.SetActive(true);
        GameObject roomIdUI = getChildByName(getChildByName(this.gameObject,"HostCanvas"),"ROOMID");
        roomIdUI.GetComponent<TextMeshProUGUI>().text = "ROOM ID: "+roomId;
        Gameplay.GetComponent<WebSocketClient>().Initiate(Endpoint);
    }

    public void startHostedGame(string Endpoint){
        Gameplay.GetComponent<WebSocketClient>().startHostedGame(roomId);
        // HostCanvas.SetActive(false);
        // Gameplay.GetComponent<WebSocketClient>().Initiate(Endpoint);
    }

    public void addPlayerEntry(string name)
    {
        Debug.Log("Adding the Player entry: " + name);
        GameObject nameObj = Instantiate(PlayerEntryUI, Vector3.zero, Quaternion.identity);
        nameObj.GetComponent<TextMeshProUGUI>().text=name;
        nameObj.transform.SetParent(playerTable.transform, false);

        GameObject nameObj1 = Instantiate(PlayerEntryUI, Vector3.zero, Quaternion.identity);
        nameObj1.GetComponent<TextMeshProUGUI>().text=name;
        GameObject parentTemp = getChildByName(getChildByName(this.gameObject,"JoinCanvas1"),"BACK");
        nameObj1.transform.SetParent(parentTemp.transform, false);
    }

    public void JoinGame(){
        canvas1.SetActive(false);
        JoinCanvas.SetActive(true);
    }

    public void EnterInRoom(){
        Debug.Log("Enterring in room : "+joinRoomId);
        JoinCanvas.SetActive(false);
        GameObject tempObj = getChildByName(this.gameObject,"JoinCanvas1");
        tempObj.SetActive(true);
        GameObject tempObj1 = getChildByName(tempObj,"ROOMID");
        tempObj1.GetComponent<TextMeshProUGUI>().text = "ROOM ID: "+joinRoomId;
        Gameplay.GetComponent<WebSocketClient>().Initiate(env.API_URL+'/'+joinRoomId);
    }




    public void setUsername(){
        this.username = selfUsername.text;
    }

    public void setjoinRoomId(){
        this.joinRoomId=TMPjoinRoomId.text;
    }

    public static string GenerateRandomEndpoint(int length = 5)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result+=(chars[Random.Range(0,chars.Length)]);
        }

        return  result.ToString();
    }

    private GameObject getChildByName(GameObject Parent,string child){
        return Parent.transform.Find(child)?.gameObject;
    }

    private void SetParent(GameObject Parent,GameObject childToBeSet){
        childToBeSet.transform.SetParent(Parent.transform,false);
    }
  
    public void startGameRemoveUI(){
        HostCanvas.SetActive(false);
        GameObject joinCanvas1 = getChildByName(this.gameObject,"JoinCanvas1");
        joinCanvas1.SetActive(false);
        Gameplay.GetComponent<Gameplay>().canTouch=true;
        __joystick.SetActive(true);
    }
    
}
