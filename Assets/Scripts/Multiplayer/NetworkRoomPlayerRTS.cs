﻿using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkRoomPlayerRTS : NetworkBehaviour{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyText = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;

    public bool isLeader;
    public bool IsLeader{
        set{
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private NetworkManagerRTS room;
    private NetworkManagerRTS Room{
        get{
            if(room != null){
                return room;
            }
            return room = NetworkManagerRTS.singleton as NetworkManagerRTS;
        }
    }

    public override void OnStartAuthority(){
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient(){
        Room.RoomPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnNetworkDestroy()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay(){
        if(!hasAuthority){
            foreach(var player in Room.RoomPlayers){
                if(player.hasAuthority){
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for(int i = 0; i<playerNameTexts.Length; i++){
            playerNameTexts[i].text = "Waiting For Player";
            playerReadyText[i].text = string.Empty;
        }

        for(int i = 0; i<Room.RoomPlayers.Count; i++){
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyText[i].text = Room.RoomPlayers[i].isReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStat(bool readyToStart){
        if(!isLeader){
            return;
        }
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName){
        DisplayName = displayName;
    }
    
    [Command]
    public void CmdReadyUp(){
        isReady = !isReady;
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame(){
        if(Room.RoomPlayers[0].connectionToClient != connectionToClient){
            return;
        }

        Room.StartGame();
    }
}

