using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Text;

[System.Serializable]
public class payLoad
{
    public string socketId;
    public string type;
    public float[] position;
    public float rotation;
    public string question;
    public string answer;
    public string name;

    public string data;
}
public class Result
{
    public int rank;
    public string socketId;
    public string name;
    public List<string> ranking;
}
public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    public Transform playerPosition;
    public GameObject playerTemp;
    private Dictionary<string, GameObject> playerMap;
    public string selfId;

    public GameObject util;

    private bool started = false;

    public Loading loading;

    public lobbyController lobbycontroller;
    public async void Initiate(string Endpoint)
    {
        loading.show();
        this.started = true;
        playerMap = new Dictionary<string, GameObject>();
        // websocket = new WebSocket("ws://localhost:8080/game/1e769");
        websocket = new WebSocket(Endpoint);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            this.setNameCall();

        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            lobbycontroller.showError(e);
        };

        websocket.OnClose += (closeCode) =>
        {
            // loading.hide();
            Debug.Log($"Connection closed with code: {closeCode}");
        };

        websocket.OnMessage += (bytes) =>
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            payLoad pl = JsonUtility.FromJson<payLoad>(jsonString);
            switch (pl.type)
            {
                case "position":
                    if (playerMap.ContainsKey(pl.socketId))
                    {
                        GameObject obj = playerMap[pl.socketId];
                        Vector3 newPosition = new Vector3(pl.position[0], pl.position[1], pl.position[2]);
                        obj.transform.position = newPosition;
                        Vector3 currentRotation = obj.transform.rotation.eulerAngles;
                        currentRotation.z = pl.rotation;
                        obj.transform.rotation = Quaternion.Euler(currentRotation);
                    }
                    else
                    {
                        Debug.Log("Player not found in map");
                    }
                    break;

                case "spawn rat":
                    util.GetComponent<Utility>().setQuestion(pl.question, pl.answer,
                        new Vector3(pl.position[0], pl.position[1], pl.position[2]));
                    break;

                case "dummy rat":
                    util.GetComponent<Utility>().spawnDummyRat(new Vector3(pl.position[0], pl.position[1], pl.position[2]), pl.answer);
                    break;

                case "self id":
                    this.selfId = pl.socketId;
                    Vector3 newSelfPosition = new Vector3(pl.position[0], pl.position[1], pl.position[2]);
                    transform.position = newSelfPosition;
                    this.GetComponent<Gameplay>().setName(pl.name);
                    playerMap.Add(this.selfId, this.gameObject);
                    loading.hide();
                    break;

                case "new player":
                    GameObject p = Instantiate(playerTemp, new Vector3(pl.position[0], pl.position[1], pl.position[2]), Quaternion.identity);
                    p.GetComponent<remoteGameplay>().selfId = pl.socketId;
                    p.GetComponent<remoteGameplay>().setLabel(pl.name);
                    playerMap.Add(pl.socketId, p);
                    Debug.Log("changing name from socket 1");
                    break;

                case "leave player":
                    if (playerMap.ContainsKey(pl.socketId))
                    {
                        playerMap[pl.socketId].GetComponent<remoteGameplay>().destoyFollowers();
                        Destroy(playerMap[pl.socketId]);
                        playerMap.Remove(pl.socketId);
                        lobbycontroller.removeEntryFromBoard(pl.name);
                    }
                    break;

                case "addRat":
                    if (playerMap.ContainsKey(pl.question))
                    {
                        if (pl.question == this.selfId)
                        {
                            this.GetComponent<Gameplay>().addRat(pl.answer);
                        }
                        else
                        {
                            GameObject obj = playerMap[pl.question];
                            obj.GetComponent<remoteGameplay>().addRat(pl.answer);
                        }
                    }
                    break;

                case "startGame":
                    lobbycontroller.startGameRemoveUI();
                    break;

                case "Error":
                    loading.hide();
                    lobbycontroller.showError(pl.data);
                    break;

                case "setName":
                    Debug.Log("setName message received");
                    if (playerMap.ContainsKey(pl.socketId))
                    {
                        if (pl.socketId == this.selfId)
                        {
                            this.GetComponent<Gameplay>().setName(pl.data);
                        }
                        else
                        {
                            GameObject obj = playerMap[pl.socketId];
                            obj.GetComponent<remoteGameplay>().setName(pl.data);
                        }
                        lobbycontroller.addPlayerEntry(pl.data);
                    }
                    break;

                case "result":
                    Result result = JsonUtility.FromJson<Result>(pl.data);
                    lobbycontroller.showResult(result.ranking, result.rank);
                    break;

                default:
                    Debug.Log("Message type not found: " + pl.type);
                    break;
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
        finally
        {

        }


    }



    void Update()
    {
        try
        {

#if (!UNITY_WEBGL || UNITY_EDITOR)
            websocket.DispatchMessageQueue();
#endif
        }
        catch
        {

        }
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

    public async void OnApplicationQuit()
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

    public async void attachRat(string str)
    {
        // this.GetComponent<WebSocketClient>().attachRat(obj.name);
        if (websocket.State == WebSocketState.Open)
        {
            try
            {
                string jsonMessage = "{"
                + "\"socketId\": \"" + this.selfId + "\","
                + "\"type\": \"addRat\","
                + "\"position\": [" + this.playerPosition.position.x + ", "
                                    + this.playerPosition.position.y + ", "
                                    + this.playerPosition.position.z + "],"
                + "\"rotation\": " + this.playerPosition.transform.rotation.eulerAngles.z + ","
                + "\"answer\": \"" + str + "\""
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

    public async void startHostedGame(string roomId)
    {
        if (websocket.State == WebSocketState.Open)
        {
            try
            {
                string jsonMessage = "{"
                    + "\"socketId\": \"" + this.selfId + "\","
                    + "\"type\": \"startGame\","
                    + "\"position\": [],"
                    + "\"rotation\": null,"
                    + "\"data\": \"" + roomId + "\","
                    + "\"answer\": \"\""
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

    public async void setNameCall()
    {
        if (websocket.State == WebSocketState.Open)
        {
            try
            {
                string username = lobbycontroller.username;

                string jsonMessage = "{"
                    + "\"socketId\": \"" + this.selfId + "\","
                    + "\"type\": \"setName\","
                    + "\"position\": [],"
                    + "\"rotation\": null,"
                    + "\"data\": \"" + username + "\","
                    + "\"answer\": \"\""
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

}
