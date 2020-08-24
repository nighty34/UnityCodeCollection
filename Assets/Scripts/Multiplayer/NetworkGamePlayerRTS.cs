using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkGamePlayerRTS : NetworkBehaviour{


    [SyncVar]
    private string displayName = "Loading...";

    private NetworkManagerRTS room;
    private NetworkManagerRTS Room{
        get{
            if(room != null){
                return room;
            }
            return room = NetworkManagerRTS.singleton as NetworkManagerRTS;
        }
    }

    public override void OnStartClient(){
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnNetworkDestroy(){
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName){
        this.displayName = displayName;
    }
}

