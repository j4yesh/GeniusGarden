using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class lobbyController : MonoBehaviour
{
    private GameObject canvas1,HostCanvas,JoinCanvas;
    public string username,joinRoomId;
    private TMP_InputField  selfUsername;

    private TMP_InputField  TMPjoinRoomId;
    public FloatingJoystick __joystick;

    public CameraFollow camerafollow;

    private string filePath;
    private Utility util;
    public GameObject Gameplay;
    public GameObject PlayerEntryUI ;

    private GameObject playerTable;

    private Loading loading;
    private string roomId = "";

    private Dictionary<string,List<GameObject>> playerInRoom;

    public SoundEffect soundeffect;

    public GameObject realtimeNameTemp;
    void Start()
    {   
        playerInRoom = new Dictionary<string, List<GameObject>>();
        canvas1 = transform.Find("1")?.gameObject;
        HostCanvas = transform.Find("HostCanvas")?.gameObject;
        JoinCanvas = transform.Find("JoinCanvas")?.gameObject;
        selfUsername = canvas1.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField >();
        TMPjoinRoomId = JoinCanvas.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField>();
        playerTable=HostCanvas.transform.Find("BAGGY")?.gameObject;
        loading = transform.Find("Loading").gameObject.GetComponent<Loading>();
        canvas1.SetActive(true);
        HostCanvas.SetActive(false);
        __joystick.SetActive(false);
        Debug.Log(env.API_URL);
        camerafollow.setBlur(true);
        selfUsername.text = GenerateRandomEndpoint();
    }

    void Update()
    {
    }

    public void hostGame(){
        roomId = GenerateRandomEndpoint();
        string Endpoint = env.API_URL+'/'+roomId+"/host";
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
        GameObject parentTemp = getChildByName(getChildByName(this.gameObject,"JoinCanvas1"),"BAGGY");
        nameObj1.transform.SetParent(parentTemp.transform, false);

        List<GameObject> twoList = new List<GameObject>();
        twoList.Add(nameObj);
        twoList.Add(nameObj1);
        playerInRoom.Add(name, twoList);
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
        loading.hide();
        if(joinRoomId=="")joinRoomId="_";
        GameObject tempObj1 = getChildByName(tempObj,"ROOMID");
        tempObj1.GetComponent<TextMeshProUGUI>().text = "ROOM ID: "+joinRoomId;
        Gameplay.GetComponent<WebSocketClient>().Initiate(env.API_URL+'/'+joinRoomId+"/join");
    }

    public void removeEntryFromBoard(string id){
        Debug.Log("remove entry called");
        if(playerInRoom.ContainsKey(id)){
            List<GameObject> toBeRemoved = playerInRoom[id];
            foreach(GameObject it in toBeRemoved){
                Destroy(it);
            }
            playerInRoom.Remove(id);
        }
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
        canvas1.SetActive(false);
        Gameplay.GetComponent<Gameplay>().canTouch=true;
        __joystick.SetActive(true);
        camerafollow.setBlur(false);
    }

    public void playPublicRoom(){
        this.startGameRemoveUI();
        Gameplay.GetComponent<WebSocketClient>().Initiate(env.API_URL+"/random/joinrandom");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void RestartScene()
    {   
        Gameplay.GetComponent<WebSocketClient>().OnApplicationQuit();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void showError(string data){
        GameObject objToInActive=getChildByName(this.gameObject,"1");
        GameObject objToInActive1=getChildByName(this.gameObject,"HostCanvas");
        GameObject objToInActive2=getChildByName(this.gameObject,"JoinCanvas");
        GameObject objToInActive3=getChildByName(this.gameObject,"JoinCanvas1");
        objToInActive.SetActive(false);
        objToInActive1.SetActive(false);
        objToInActive2.SetActive(false);
        objToInActive3.SetActive(false);

        GameObject errorCanvas = getChildByName(this.gameObject,"Error");
        errorCanvas.SetActive(true);
        GameObject textObj = getChildByName(getChildByName(errorCanvas, "BAGGY"),"MSG");
        textObj.GetComponent<TextMeshProUGUI>().text = data;    

    }


    public void showResult(List<string> ranking, int rank)
    {   
        Gameplay.GetComponent<Gameplay>().canTouch=false;
        camerafollow.setBlur(true);
        GameObject resultObj = getChildByName(this.gameObject, "Result");
        string star = "";

        if (ranking.Count >= 1 && ranking[0] == username)
        {
            star = "3_star";
        }
        else if (ranking.Count >= 2 && ranking[1] == username)
        {
            star = "2_star";
        }
        else if (ranking.Count >= 3 && ranking[2] == username)
        {
            star = "1_star"; 
        }
        else
        {
            Debug.Log("ranking list overflow.");
        }

        GameObject starObject = getChildByName(resultObj, star);
        GameObject Board = getChildByName(resultObj, "BAGGY");

        // resultObj.Transform.position.y = 2080;

        // resultObj.transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);


        resultObj.SetActive(true);

        starObject.transform.localScale = Vector3.zero; 
        starObject.SetActive(true); 
        starObject.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);  


        for (int i = 0; i < ranking.Count; i++)
        {
            GameObject rankTextObj = Instantiate(PlayerEntryUI, Vector3.zero, Quaternion.identity);
            rankTextObj.GetComponent<TextMeshProUGUI>().text = (ranking.Count - i).ToString() + " " + ranking[i];
            rankTextObj.transform.SetParent(Board.transform, false);
        }
    }

    public void realtimeGameRanking(List<string> rankingList)
    {
        if (rankingList == null || rankingList.Count == 0)
        {
            Debug.LogWarning("Ranking List is empty or null");
            return;
        }

        Debug.Log("Ranking List called: " + rankingList[0]);

        // Get the ranking object
        GameObject rankingObj = getChildByName(this.gameObject, "Ranking");

        // Clear the current ranking children
        foreach (Transform child in rankingObj.transform)
        {
            if(child.gameObject.tag!="playerLabel")Destroy(child.gameObject);
        }

        // Create new ranking based on the list
        for (int i = 0; i < rankingList.Count; i++)
        {
            GameObject singleRank = Instantiate(realtimeNameTemp, rankingObj.transform);
            singleRank.tag="Snake";
            // Set the TextMeshPro for the rank
            TextMeshProUGUI textmeshpro = singleRank.GetComponentInChildren<TextMeshProUGUI>();

            if (textmeshpro != null)
            {
                textmeshpro.text = " " + (i + 1).ToString() + ". " + rankingList[i];
                Debug.Log("TextMeshPro ranking: " + textmeshpro.text);
            }
            else
            {
                Debug.LogError("TextMeshPro component not found in singleRank");
            }
        }
    }


    
}
