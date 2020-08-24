using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class NetworkManagerRTS : NetworkManager{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerRTS roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerRTS gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem;
    [SerializeField] private GameObject gameMaster; 

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerRTS> RoomPlayers {get;} = new List<NetworkRoomPlayerRTS>();
    public List<NetworkGamePlayerRTS> GamePlayers {get;} = new List<NetworkGamePlayerRTS>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient(){
        var SpawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        
        foreach (var prefab in SpawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn){
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn){
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn){
        if(numPlayers >= maxConnections){
            conn.Disconnect();
            return;
        }
        
        if(SceneManager.GetActiveScene().path != menuScene){
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn){
        if(SceneManager.GetActiveScene().path == menuScene){
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerRTS roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn){
        if(conn.identity != null){
            var player = conn.identity.GetComponent<NetworkRoomPlayerRTS>();
            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer(){

        OnServerStopped?.Invoke();
        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState(){
        foreach(var player in RoomPlayers){
            player.HandleReadyToStat(IsReadyToStart());
        }
    }

    private bool IsReadyToStart(){
        if(numPlayers < minPlayers) {
            return false;
        }

        foreach(var player in RoomPlayers){
            if(!player.isReady){
                return false;
            }
        }
        return true;
    }

    public void StartGame(){
        if(SceneManager.GetActiveScene().path == menuScene){
            if(!IsReadyToStart()){
                return;
            }
            ServerChangeScene("TestScene");
        }

    }

    public override void ServerChangeScene(string newSceneName){
        if(SceneManager.GetActiveScene().path == menuScene){
            print("RoomCount: " + RoomPlayers.Count);
            for(int i = RoomPlayers.Count - 1; i >= 0; i--){
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
            

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);   
                //NetworkServer.Destroy(conn.identity.gameObject);             
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName){
        GameObject gameMasterInstance = Instantiate(gameMaster);
        NetworkServer.Spawn(gameMasterInstance);
        
        GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
        NetworkServer.Spawn(playerSpawnSystemInstance);
    }

    public override void OnServerReady(NetworkConnection conn){
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
