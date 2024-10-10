using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public string email;
    public string otp;
    public bool active;
    public int games;
    public int correct;
    public int wrong;
    public float acceptance;
}

[System.Serializable]
public class UserDataList
{
    public List<UserData> users;
}

[System.Serializable]
public class LoginData
{
    public string username;
    public string password;

    public LoginData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

public class RestAPI : MonoBehaviour
{
    private Leaderboard leaderboard;

    private string cookie;

    public TMP_InputField username, password;


    private Loading loading;
    private GameObject loginPageObj;

    private TextMeshProUGUI menuUsername;
    void Start()
    {
        GameObject oneObj = lobbyController.getChildByName(this.gameObject, "1");
        menuUsername = lobbyController.getChildByName(oneObj, "USERNAME").GetComponent<TextMeshProUGUI>();

        menuUsername.text = "Loading";

        getMenuUsername();

        this.leaderboard = lobbyController.getChildByName(this.gameObject, "Leaderboard")
            .GetComponent<Leaderboard>();

        StartCoroutine(GetRequest(env.API_URL1 + "/leaderboard"));

        SaveData saveData = SaveManager.LoadGameState();
        if (saveData.cookie != null)
        {
            cookie = saveData.cookie;
            Debug.Log("cookie: " + cookie);
        }

        loading = transform.Find("Loading").gameObject.GetComponent<Loading>();


    }

    async Task  getMenuUsername()
{
    using var client = new HttpClient();

        // Correct the URL with protocol (http://)
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/getusername");

        // Add Authorization header with the Bearer token
        request.Headers.Add("Authorization", this.cookie);

        // Send the request and get the response
        var response = await client.SendAsync(request);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        // Output the response content (username in this case)
        var responseBody = await response.Content.ReadAsStringAsync();
        Debug.Log("responsebody: "+responseBody);
}



    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);

                UserDataList UDL = JsonUtility.FromJson<UserDataList>("{\"users\":" + webRequest.downloadHandler.text + "}");


                for (int i = 0; i < UDL.users.Count; i++)
                {
                    UserData user = UDL.users[i];

                    leaderboard.addRow(
                        (i + 1).ToString(),
                        user.username,
                        user.games.ToString(),
                        user.acceptance.ToString("F2")
                    );
                }

                leaderboard.hideLoading();
            }
        }
    }

    public void LoginButton()
    {
        StartCoroutine(Login());
    }

    public IEnumerator Login()
    {
        string uri = env.API_URL1 + "/login";
        string jsonPayload = JsonUtility.ToJson(new LoginData(username.text, password.text));

        UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);

        webRequest.SetRequestHeader("Content-Type", "application/json");

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        loading.show();

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + webRequest.error);
            this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
            this.gameObject.GetComponent<lobbyController>().showError(webRequest.error);
        }
        else
        {
            Debug.Log("Response: " + webRequest.downloadHandler.text);

            this.cookie = webRequest.downloadHandler.text;

            SaveData saveData = SaveManager.LoadGameState();
            saveData.cookie = this.cookie;
            SaveManager.SaveGameState(saveData);
            this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
            StartCoroutine(Successfull("User Logged in Successfully."));

        }

        loading.hide();
    }

    IEnumerator Successfull(string msg)
    {
        GameObject successObj = lobbyController.getChildByName(this.gameObject, "Success");
        successObj.SetActive(true);
        GameObject textObj = lobbyController.getChildByName(successObj, "text");
        textObj.GetComponent<TextMeshProUGUI>().text = msg;
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<lobbyController>().RestartScene();
    }

}
