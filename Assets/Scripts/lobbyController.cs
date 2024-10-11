using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;


public class lobbyController : MonoBehaviour
{
    private GameObject canvas1, HostCanvas, JoinCanvas;
    public string username, joinRoomId;
    private TMP_InputField selfUsername;

    private TMP_InputField TMPjoinRoomId;
    public FloatingJoystick __joystick;

    public CameraFollow camerafollow;

    private string filePath;
    private Utility util;
    public GameObject Gameplay;
    public GameObject PlayerEntryUI;

    private GameObject playerTable;

    private Loading loading;
    private string roomId = "";

    private Dictionary<string, List<GameObject>> playerInRoom;

    public SoundEffect soundeffect;

    public string gameState;
    public GameObject realtimeNameTemp;
    void Start()
    {   
        this.gameState="menu";
        playerInRoom = new Dictionary<string, List<GameObject>>();
        canvas1 = transform.Find("1")?.gameObject;
        HostCanvas = transform.Find("HostCanvas")?.gameObject;
        JoinCanvas = transform.Find("JoinCanvas")?.gameObject;
        selfUsername = canvas1.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField>();
        TMPjoinRoomId = JoinCanvas.transform.Find("NAME")?.gameObject.GetComponent<TMP_InputField>();
        playerTable = HostCanvas.transform.Find("BAGGY")?.gameObject;
        loading = transform.Find("Loading").gameObject.GetComponent<Loading>();
        canvas1.SetActive(true);
        HostCanvas.SetActive(false);
        __joystick.SetActive(false);
        Debug.Log(env.API_URL);
        camerafollow.setBlur(true);
        this.LoadData();
        // selfUsername.text = GenerateRandomEndpoint();
        GameObject rankingObj = getChildByName(this.gameObject,"Ranking");
        rankingObj.SetActive(false);
        GameObject leaderboardObj = getChildByName(this.gameObject,"Leaderboard");
        leaderboardObj.SetActive(true);
    }


    public void hostGame()
    {   
        GameObject leaderboard=getChildByName(this.gameObject,"Leaderboard");
        leaderboard.SetActive(false);

        roomId = GenerateRandomEndpoint();
        string Endpoint = env.API_URL + '/' + roomId + "/host";
        Debug.Log(Endpoint);
        canvas1.SetActive(false);
        HostCanvas.SetActive(true);
        GameObject roomIdUI = getChildByName(getChildByName(this.gameObject, "HostCanvas"), "ROOMID");
        roomIdUI.GetComponent<TextMeshProUGUI>().text = "ROOM ID: " + roomId;
        Gameplay.GetComponent<WebSocketClient>().Initiate(Endpoint);

    }

    public void startHostedGame(string Endpoint)
    {
        Gameplay.GetComponent<WebSocketClient>().startHostedGame(roomId);
        // HostCanvas.SetActive(false);
        // Gameplay.GetComponent<WebSocketClient>().Initiate(Endpoint);
    }

    public void addPlayerEntry(string name)
    {
        Debug.Log("Adding the Player entry: " + name);
        GameObject nameObj = Instantiate(PlayerEntryUI, Vector3.zero, Quaternion.identity);
        nameObj.GetComponent<TextMeshProUGUI>().text = name;
        nameObj.transform.SetParent(playerTable.transform, false);

        GameObject nameObj1 = Instantiate(PlayerEntryUI, Vector3.zero, Quaternion.identity);
        nameObj1.GetComponent<TextMeshProUGUI>().text = name;
        GameObject parentTemp = getChildByName(getChildByName(this.gameObject, "JoinCanvas1"), "BAGGY");
        nameObj1.transform.SetParent(parentTemp.transform, false);

        List<GameObject> twoList = new List<GameObject>();
        twoList.Add(nameObj);
        twoList.Add(nameObj1);
        playerInRoom.Add(name, twoList);
    }


    public void JoinGame()
    {   
        GameObject leaderboard= getChildByName(this.gameObject,"Leaderboard");
        leaderboard.SetActive(false);
        canvas1.SetActive(false);
        JoinCanvas.SetActive(true);
    }

    public void EnterInRoom()
    {
        Debug.Log("Enterring in room : " + joinRoomId);
        JoinCanvas.SetActive(false);
        GameObject tempObj = getChildByName(this.gameObject, "JoinCanvas1");
        tempObj.SetActive(true);
        loading.hide();
        if (joinRoomId == "") joinRoomId = "_";
        GameObject tempObj1 = getChildByName(tempObj, "ROOMID");
        tempObj1.GetComponent<TextMeshProUGUI>().text = "ROOM ID: " + joinRoomId;
        Gameplay.GetComponent<WebSocketClient>().Initiate(env.API_URL + '/' + joinRoomId + "/join");
    }

    public void removeEntryFromBoard(string id)
    {
        Debug.Log("remove entry called");
        if (playerInRoom.ContainsKey(id))
        {
            List<GameObject> toBeRemoved = playerInRoom[id];
            foreach (GameObject it in toBeRemoved)
            {
                Destroy(it);
            }
            playerInRoom.Remove(id);
        }
    }


    public void setUsername()
    {
        this.username = selfUsername.text;
        SaveData();
    }

    public void setjoinRoomId()
    {
        this.joinRoomId = TMPjoinRoomId.text;
    }

    public static string GenerateRandomEndpoint(int length = 5)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += (chars[Random.Range(0, chars.Length)]);
        }

        return result.ToString();
    }

    public static GameObject getChildByName(GameObject Parent, string child)
    {
        return Parent.transform.Find(child)?.gameObject;
    }

    private void SetParent(GameObject Parent, GameObject childToBeSet)
    {
        childToBeSet.transform.SetParent(Parent.transform, false);
    }

    public void startGameRemoveUI()
    {   
        this.HideAllCanvas();
        GameObject rankingObj = getChildByName(this.gameObject,"Ranking");
        rankingObj.SetActive(true);
        GameObject settingIcon = getChildByName(this.gameObject, "SettingIcon");
        settingIcon.SetActive(true);
        GameObject exitIcon = getChildByName(this.gameObject, "ExitIcon");
        exitIcon.SetActive(true);
        GameObject resultObj = getChildByName(this.gameObject, "Result");
        resultObj.SetActive(false);


        this.gameState="gameplay";
        HostCanvas.SetActive(false);
        GameObject joinCanvas1 = getChildByName(this.gameObject, "JoinCanvas1");
        joinCanvas1.SetActive(false);
        canvas1.SetActive(false);
        Gameplay.GetComponent<Gameplay>().canTouch = true;
        __joystick.SetActive(true);
        camerafollow.setBlur(false);

        
    }

    public void playPublicRoom()
    {
        this.startGameRemoveUI();
         GameObject leaderboard= getChildByName(this.gameObject,"Leaderboard");
        leaderboard.SetActive(false);
        Gameplay.GetComponent<WebSocketClient>().Initiate(env.API_URL + "/random/joinrandom");
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

    public void showError(string data)
    {
this.   HideAllCanvas();


        GameObject errorCanvas = getChildByName(this.gameObject, "Error");
        errorCanvas.SetActive(true);
        GameObject textObj = getChildByName(getChildByName(errorCanvas, "BAGGY"), "MSG");
        textObj.GetComponent<TextMeshProUGUI>().text = data;

    }


    public void showResult(List<string> ranking, int rank)
    {   

        GameObject settingIcon = getChildByName(this.gameObject, "SettingIcon");
        settingIcon.SetActive(false);
        GameObject exitIcon = getChildByName(this.gameObject, "ExitIcon");
        exitIcon.SetActive(false);

        GameObject rankingObj = getChildByName(this.gameObject,"Ranking");
        rankingObj.SetActive(false);

        Gameplay.GetComponent<Gameplay>().canTouch = false; 
        camerafollow.setBlur(true);
        GameObject resultObj = getChildByName(this.gameObject, "Result");
        GameObject baggyObj = getChildByName(resultObj, "BAGGY");
        this.deleteChild(baggyObj);
        string star = "";

        if (rank == 1)
        {
            star = "3_star";
        }
        else if (rank == 2)
        {
            star = "2_star";
        }
        else if (rank == 3)
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

        GameObject rankingObj = getChildByName(this.gameObject, "Ranking");

          foreach (Transform child in rankingObj.transform)
        {   
            if(child.gameObject.name!="Rank"){
            Destroy(child.gameObject);}
        }

        foreach (Transform child in rankingObj.transform)
        {
            if (child.gameObject.tag != "playerLabel") Destroy(child.gameObject);
        }

        for (int i = 0; i < rankingList.Count; i++)
        {
            GameObject singleRank = Instantiate(realtimeNameTemp, rankingObj.transform);
            singleRank.tag = "Snake";
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

    public void Settings()
    {    GameObject leaderboard= getChildByName(this.gameObject,"Leaderboard");
        leaderboard.SetActive(false);
        
        GameObject settingIcon = getChildByName(this.gameObject,"SettingIcon");
        settingIcon.SetActive(false);

GameObject ExitIcon = getChildByName(this.gameObject,"ExitIcon");
        ExitIcon.SetActive(false);
        
        Gameplay.GetComponent<Gameplay>().canTouch=false;
        GameObject settingObj = getChildByName(this.gameObject, "Setting");
        GameObject menuObj = getChildByName(this.gameObject, "1");
        menuObj.SetActive(false);
        settingObj.SetActive(true);

    }


    void SaveData()
    {
        SaveData saveData = SaveManager.LoadGameState();
        saveData.username = this.username;
        SaveManager.SaveGameState(saveData);
    }

    void LoadData()
    {
        SaveData saveData = SaveManager.LoadGameState();
        if (saveData != null)
        {
            this.username = saveData.username;
                selfUsername.text = this.username;

        
            GameObject settingObj = getChildByName(this.gameObject, "Setting");

                GameObject musicObj = getChildByName(settingObj, "Music");
                GameObject sfxObj = getChildByName(settingObj, "SFX");
                
                    musicObj.GetComponent<Toggle>().isOn = saveData.bgmAllowed;
                    sfxObj.GetComponent<Toggle>().isOn = saveData.sfxAllowed;

                soundeffect.setSFX(saveData.sfxAllowed);
                soundeffect.setBGM(saveData.bgmAllowed);
            
                Debug.Log("music status "+saveData.bgmAllowed+" "+saveData.sfxAllowed);
        }else{
            Debug.Log("saved data not found. ");
        }
    }

    public void showRules(){
        GameObject settingObj = getChildByName(this.gameObject,"Setting");    
        GameObject ruleScroll = getChildByName(this.gameObject,"RulesScroll");
        settingObj.SetActive(false);
        ruleScroll.SetActive(true);
    }
    public void hideRules(){
        GameObject settingObj = getChildByName(this.gameObject,"Setting");    
        GameObject ruleScroll = getChildByName(this.gameObject,"RulesScroll");
        settingObj.SetActive(true);
        ruleScroll.SetActive(false);
    }
    public void showPolicy(){
        GameObject settingObj = getChildByName(this.gameObject,"Setting");    
        GameObject policyScroll = getChildByName(this.gameObject,"PolicyScroll");
        settingObj.SetActive(false);
        policyScroll.SetActive(true);
    }
    public void hidePolicy(){
        GameObject settingObj = getChildByName(this.gameObject,"Setting");    
        GameObject policyScroll = getChildByName(this.gameObject,"PolicyScroll");
        settingObj.SetActive(true);
        policyScroll.SetActive(false);
    }

    public void backFromSettings(){
        if(this.gameState=="menu"){
            this.RestartScene();
        }else if(this.gameState=="gameplay"){
            GameObject settingIcon = getChildByName(this.gameObject,"SettingIcon");
        settingIcon.SetActive(true);

GameObject ExitIcon = getChildByName(this.gameObject,"ExitIcon");
        ExitIcon.SetActive(true);
            Gameplay.GetComponent<Gameplay>().canTouch=true;
            GameObject settingObj = getChildByName(this.gameObject,"Setting");
            settingObj.SetActive(false);

             GameObject notificObj = getChildByName(this.gameObject,"Notification");
        notificObj.SetActive(false);
        }
    }

    public void exitGameplay(){
        camerafollow.setBlur(true);
        
        GameObject confirmation = getChildByName(this.gameObject,"Confirmation");
        Gameplay.GetComponent<Gameplay>().canTouch=false;
        GameObject textObject = getChildByName(confirmation, "text");
        textObject.GetComponent<TextMeshProUGUI>().text = "Are you sure you want to exit the game?";
        confirmation.SetActive(true);
    }
    public void continueGameplay(){
        camerafollow.setBlur(false);

        GameObject settingIcon = getChildByName(this.gameObject,"SettingIcon");
        settingIcon.SetActive(true);

GameObject ExitIcon = getChildByName(this.gameObject,"ExitIcon");
        ExitIcon.SetActive(true);

        GameObject confimation = getChildByName(this.gameObject,"Confirmation");
        confimation.SetActive(false);
        Gameplay.GetComponent<Gameplay>().canTouch=true;
    }

    public void callRematch(){
        
        Gameplay.GetComponent<WebSocketClient>().callRematch();
    }

    public void HideAllCanvas()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public static void HideAllCanvas(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    public void deleteChild(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void showNotification(string message){
        GameObject settingIcon = getChildByName(this.gameObject, "SettingIcon");
        settingIcon.SetActive(false);
        GameObject exitIcon = getChildByName(this.gameObject, "ExitIcon");
        exitIcon.SetActive(false);

        GameObject notificObj = getChildByName(this.gameObject,"Notification");
        notificObj.SetActive(true);

        GameObject textObj = getChildByName(notificObj,"text");
        textObj.GetComponent<TextMeshProUGUI>().text=message;
                Gameplay.GetComponent<Gameplay>().canTouch = false; 

    }

    public void showLogin(){
        GameObject leadObj = getChildByName(this.gameObject,"Leaderboard");
        GameObject MenuObj = getChildByName(this.gameObject,"1");
        MenuObj.SetActive(false);
        leadObj.SetActive(false);
        GameObject loginObj = getChildByName(this.gameObject,"Login");
        loginObj.SetActive(true);
    }

}
