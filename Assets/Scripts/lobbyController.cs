using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lobbyController : MonoBehaviour
{
    private GameObject canvas1,canvas2;
    public string username;
    private TMP_InputField  tmpro;

    private string filePath;
    
    public GameObject Gameplay;

    void Start()
    {   
        canvas1 = transform.Find("1")?.gameObject;
        canvas2 = transform.Find("2")?.gameObject;
        tmpro = canvas1.transform.Find("NAME").gameObject.GetComponent<TMP_InputField >();
        canvas1.SetActive(true);
        canvas2.SetActive(false);
        
        Debug.Log(env.API_URL);
    }

    void Update()
    {
    }

    public void hostGame(){
        string roomId = GenerateRandomEndpoint();
        string Endpoint = env.API_URL+'/'+roomId;
        Debug.Log(Endpoint);
        canvas1.SetActive(false);
        Gameplay.GetComponent<WebSocketClient>().Initiate(Endpoint);
    }

    public void setUsername(){
        this.username = tmpro.text;
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

  
}
