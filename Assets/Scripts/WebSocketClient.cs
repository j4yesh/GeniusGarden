using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;


[System.Serializable]
public class payLoad
{
    public string socketId;
    public string type;
    public float[] position;
    public float rotation;
    public string question;

    public string answer;
}

public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    public Transform playerPosition;
    public GameObject playerTemp;
    private Dictionary<string, GameObject> playerMap;
    public string selfId;

    public GameObject util;
    async void Start()
    {
        playerMap = new Dictionary<string, GameObject>();
        websocket = new WebSocket("ws://localhost:8080/game");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (closeCode) =>
        {
            Debug.Log($"Connection closed with code: {closeCode}");
        };

        websocket.OnMessage += (bytes) =>
        {

            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);

            payLoad pl = JsonUtility.FromJson<payLoad>(jsonString);
            if (pl.type == "position")
            {
                if (playerMap.ContainsKey(pl.socketId))
                {
                    GameObject obj = playerMap[pl.socketId];
                    Vector3 newPosition = new Vector3(pl.position[0], pl.position[1], pl.position[2]);
                    obj.transform.position = newPosition;
                    Vector3 currentRotation = obj.transform.rotation.eulerAngles;
                    currentRotation.z = pl.rotation;
                    obj.transform.rotation = Quaternion.Euler(currentRotation);
                }else{
                    Debug.Log("Player not found in map");
                }
            }
            else if(pl.type=="spawn rat"){
                // new Vector3(this.headPos.position.x+Random.Range(4,-4),this.headPos.position.y+Random.Range(4,-4), 0)
                util.GetComponent<Utility>().setQuestion(pl.question,pl.answer
                ,new Vector3(pl.position[0],pl.position[1],pl.position[2]));
            }else if(pl.type=="dummy rat"){
                util.GetComponent<Utility>().spawnDummyRat(new Vector3(pl.position[0],pl.position[1],pl.position[2])
                ,pl.answer);
            }
            else if (pl.type == "self id")
            {
                this.selfId = pl.socketId;
                Vector3 newPosition = new Vector3(pl.position[0],pl.position[1],pl.position[2]);
                transform.position = newPosition;
            }
            else if (pl.type == "new player")
            {
                GameObject p = Instantiate(playerTemp,
                  new Vector3(pl.position[0],
                  pl.position[1], pl.position[2]), Quaternion.identity);

                playerMap.Add(pl.socketId, p);
            }
            else if (pl.type == "leave player")
            {
                if (playerMap.ContainsKey(pl.socketId))
                {
                    Destroy(playerMap[pl.socketId]);
                    playerMap.Remove(pl.socketId);
                }
            }
            Debug.Log("OnMessage! " + jsonString);
        };

        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.01f);

        try
        {
            await websocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection error: {ex.Message}");
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            try
            {
                string jsonMessage = "{"
                + "\"socketId\": \"" + this.selfId + "\","
                + "\"type\": \"position\","
                + "\"position\": [" + this.playerPosition.position.x + ", " +
                this.playerPosition.position.y + ", " + this.playerPosition.position.z + "],"
                + "\"rotation\": " + this.playerPosition.transform.rotation.eulerAngles.z
                + "}";


                await websocket.SendText(jsonMessage);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Send error: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Current state: " + websocket.State);
        }
    }

    private async void OnApplicationQuit()
    {
        Debug.Log("Application quitting. Closing WebSocket...");
        if (websocket != null)
        {
            try
            {
                await websocket.Close();
                Debug.Log("WebSocket closed.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error closing WebSocket: {ex.Message}");
            }
        }
    }
}
