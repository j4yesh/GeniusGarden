using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

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

public class RestAPI : MonoBehaviour
{   
    public Leaderboard leaderboard;

    void Start()
    {
        this.leaderboard = lobbyController.getChildByName(this.gameObject, "Leaderboard")
            .GetComponent<Leaderboard>();
        
        StartCoroutine(GetRequest(env.API_URL1 + "/leaderboard"));
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
            }
        }
    }
}
