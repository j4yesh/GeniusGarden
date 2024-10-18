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

[System.Serializable]
public class SignUpData
{
    public string username;
    public string password;
    public string email;
    public string otp;

    public SignUpData(string username, string password, string email, string otp)
    {
        this.username = username;
        this.password = password;
        this.email = email;
        this.otp = otp;
    }
}
[System.Serializable]
public class otpData
{
    public string email;

    public otpData(string email)
    {
        this.email = email;
    }
}
[System.Serializable]
public class joinDTO
{
    public string roomId;
    public joinDTO(string roomId)
    {
        this.roomId = roomId;
    }
}

[System.Serializable]
public class GameResult
{
    public string id;
    public string username;
    public string roomId;
    public int rank;
    public int correct;
    public int wrong;
    public float acceptance;
    public string time;
    public string conKey;
    public string duration;
}

[System.Serializable]
public class GameResultList
{
    public GameResult[] results;
}

[System.Serializable]
public class UserDTO
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
public class RestAPI : MonoBehaviour
{
    private Leaderboard leaderboard;

    private string cookie;

    public TMP_InputField username, password;


    private Loading loading;
    private GameObject loginPageObj;

    private TextMeshProUGUI menuUsername;
    private GameObject verificObj;

    void Start()
    {
        GameObject oneObj = lobbyController.getChildByName(this.gameObject, "1");
        // menuUsername = lobbyController.getChildByName(oneObj, "USERNAME").GetComponent<TextMeshProUGUI>();

        // menuUsername.text = "Loading";

        GameObject usernameObj = lobbyController.getChildByName( lobbyController.getChildByName(this.gameObject,"Profile")
             ,"username");
        // usernameObj.GetComponent<TextMeshProUGUI>().text=responseBody;
        menuUsername = usernameObj.GetComponent<TextMeshProUGUI>();
        menuUsername.text = "Loading";


        this.leaderboard = lobbyController.getChildByName(this.gameObject, "Leaderboard")
            .GetComponent<Leaderboard>();


        SaveData saveData = SaveManager.LoadGameState();
        if (saveData.cookie != null)
        {
            cookie = saveData.cookie;
            Debug.Log(" Cookie: " + cookie);
        }
        verificObj = lobbyController.getChildByName(this.gameObject, "SendOTP");

        loading = transform.Find("Loading").gameObject.GetComponent<Loading>();


        StartCoroutine(GetRequest(env.API_URL1 + "/leaderboard"));
        getMenuUsername();
    }


    public void showOtpVerificatiion()
    {
            this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
        verificObj.SetActive(true);
    }

    public void getOTP()
    {
        loading.show();
        GameObject emailObj = lobbyController.getChildByName(verificObj, "EMAIL");
        emailObj.SetActive(false);
        getOtpReq();
    }

    async Task getOtpReq()
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, env.API_URL1 + "/getotp");
            string email = lobbyController.getChildByName(verificObj, "EMAIL").GetComponent<TMP_InputField>().text;
            string jsonPayLoad = JsonUtility.ToJson(new otpData(email));
            Debug.Log("jsongPayload" + jsonPayLoad);
            request.Content = new StringContent(jsonPayLoad, System.Text.Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log("Response: " + responseBody);

            // this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
            lobbyController.getChildByName(this.verificObj, "EMAIL").SetActive(false);
            lobbyController.getChildByName(this.verificObj, "OTP").SetActive(true);
            lobbyController.getChildByName(this.verificObj, "USERNAME").SetActive(true);
            lobbyController.getChildByName(this.verificObj, "PASS").SetActive(true);
            lobbyController.getChildByName(this.verificObj, "SUBMITEMAIL").SetActive(false);
            lobbyController.getChildByName(this.verificObj, "SUBMITOTP").SetActive(true);
            loading.hide();




            menuUsername.text = responseBody;
        }
        catch (HttpRequestException e)
        {

            Debug.LogError("Request error: " + e.Message);
            this.gameObject.GetComponent<lobbyController>().showError(e.Message);

        }
    }

    public void signUp()
    {
        loading.show();
            this.gameObject.GetComponent<lobbyController>().HideAllCanvas();

        signUpReq();
    }
    async Task signUpReq()
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, env.API_URL1 + "/register");
            string username = lobbyController.getChildByName(verificObj, "USERNAME").GetComponent<TMP_InputField>().text;
            string password = lobbyController.getChildByName(verificObj, "PASS").GetComponent<TMP_InputField>().text;
            string otp = lobbyController.getChildByName(verificObj, "OTP").GetComponent<TMP_InputField>().text;
            string email = lobbyController.getChildByName(verificObj, "EMAIL").GetComponent<TMP_InputField>().text;
            string jsonPayLoad = JsonUtility.ToJson(new SignUpData(username, password, email, otp));

            request.Content = new StringContent(jsonPayLoad, System.Text.Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            loading.hide();

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log("Response: " + responseBody);

            StartCoroutine(Successfull(responseBody));
        }
        catch (HttpRequestException e)
        {
            menuUsername.text = "Please Login";

            menuUsername.color = Color.red;
            Debug.LogError("Request error: " + e.Message);

        }
    }



    async Task getMenuUsername()
{
    try
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, env.API_URL1 + "/getusername");

        if (!string.IsNullOrEmpty(this.cookie))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.cookie);
        }
        else
        {
            Debug.LogError("Token is missing or invalid.");
            menuUsername.text = "Please Login";
            menuUsername.color = Color.red;
            return;
        }

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        UserDTO user = JsonUtility.FromJson<UserDTO>(responseBody);

        SaveData saveData = SaveManager.LoadGameState();
        saveData.username = user.username;
        SaveManager.SaveGameState(saveData);
        this.gameObject.GetComponent<lobbyController>().username = user.username;
        Debug.Log(user);
        Debug.Log("username: " + user.username);
        menuUsername.text = user.username;
        string data = GenerateFormattedString(user);
        Debug.Log(data);
        GameObject profileObj = lobbyController.getChildByName(this.gameObject,"Profile");
        GameObject dataObj = lobbyController.getChildByName(profileObj,"data");
        dataObj.GetComponent<TextMeshProUGUI>().text = data;
    }
    catch (HttpRequestException e)
    {
        menuUsername.text = "Please Login";
        menuUsername.color = Color.red;
        this.gameObject.GetComponent<lobbyController>().username = lobbyController.GenerateRandomEndpoint();

        Debug.LogError("Request error: " + e.Message);
    }
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
        this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
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

    public void logOut()
    {
        lobbyController.getChildByName(this.gameObject, "Setting").SetActive(false);
        this.gameObject.GetComponent<lobbyController>().username = lobbyController.GenerateRandomEndpoint();
        SaveData saveData = SaveManager.LoadGameState();
        saveData.cookie = null;
        SaveManager.SaveGameState(saveData);
        StartCoroutine(Successfull("LogOut Successfully"));
    }

    public void hostGame()
    {
        loading.show();
            this.gameObject.GetComponent<lobbyController>().HideAllCanvas();
        hostGameReq();
    }

    async Task hostGameReq()
    {
        try
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, env.API_URL1 + "/getroom");

            if (!string.IsNullOrEmpty(this.cookie))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.cookie);
            }
            else
            {
                Debug.LogError("Token is missing or invalid.");
                this.gameObject.GetComponent<lobbyController>().showError("Please Login");

                return;
            }
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            loading.hide();
            var roomId = await response.Content.ReadAsStringAsync();
            this.gameObject.GetComponent<lobbyController>().hostGame(roomId);
        }
        catch (HttpRequestException e)
        {
            loading.hide();
            this.gameObject.GetComponent<lobbyController>().showError(e.Message);
            Debug.LogError("Request error: " + e.Message);
        }
    }

    public async Task joinGameReq(string username, string roomId)
    {
        loading.show();

        try
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, env.API_URL1 + "/joinroom");

            // Prepare payload and set content type to application/json
            string jsonPayload = JsonUtility.ToJson(new joinDTO(roomId));
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Set Authorization header with the Bearer token if available
            if (!string.IsNullOrEmpty(this.cookie))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.cookie);
            }
            else
            {
                Debug.LogError("Token is missing or invalid.");
                this.gameObject.GetComponent<lobbyController>().showError("Please Login");
                return;
            }

            // Send the request and check response status
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                loading.hide();
                // Assuming you may want to parse the response if needed
                string responseBody = await response.Content.ReadAsStringAsync();

                // Call the callback for successfully joining the game
                this.gameObject.GetComponent<lobbyController>().callbackJoinGame(roomId);
            }
            else
            {
                // Handle non-success status codes
                string errorResponse = await response.Content.ReadAsStringAsync();
                loading.hide();
                Debug.LogError($"Error: {response.StatusCode}, Response: {errorResponse}");
                this.gameObject.GetComponent<lobbyController>().showError("Failed to join the game. Try again.");
            }
        }
        catch (HttpRequestException e)
        {
            loading.hide();
            this.gameObject.GetComponent<lobbyController>().showError(e.Message);
            Debug.LogError("Request error: " + e.Message);
        }
    }

    public void joinPublic()
    {
        loading.show();
        this.publicRoomReq();
    }


    async Task publicRoomReq()
    {
        try
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, env.API_URL1 + "/getpublic");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            loading.hide();
            var roomId = await response.Content.ReadAsStringAsync();
            this.gameObject.GetComponent<lobbyController>().publicRoom(roomId);
        }
        catch (HttpRequestException e)
        {
            loading.hide();
            this.gameObject.GetComponent<lobbyController>().showError(e.Message);
            Debug.LogError("Request error: " + e.Message);
        }
    }

    public void callPerformance()
    {
        this.performance();
    }

    public async Task performance()
    {
        loading.show();
        this.gameObject.GetComponent<lobbyController>().HideAllCanvas();

        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, env.API_URL1 + "/getresult");

            if (!string.IsNullOrEmpty(this.cookie))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.cookie);
            }
            else
            {
                Debug.LogError("Token is missing or invalid.");
                this.gameObject.GetComponent<lobbyController>().showError("Please Login");
                loading.hide();
                return;
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();


            var res = await response.Content.ReadAsStringAsync();
            GameResultList resultList = JsonUtility.FromJson<GameResultList>("{\"results\":" + res + "}");

            this.gameObject.GetComponent<lobbyController>().showPerformance(resultList);
            loading.hide();
        }
        catch (HttpRequestException e)
        {
            loading.hide();
            this.gameObject.GetComponent<lobbyController>().showError(e.Message);
            Debug.LogError("Request error: " + e.Message);
        }
    }

    public static string GenerateFormattedString(UserDTO user)
    {
        return $"Correct Answers: {user.correct} \nWrong Answers: {user.wrong} \nAcceptance Rate: {user.acceptance:F2} \nGames Played: {user.games}";
    }
}
