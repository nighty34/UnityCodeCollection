using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class AbstractPhase : NetworkBehaviour{

    private string phaseName = null;
    public string Name {protected set{phaseName = value;} get {return phaseName;} }
    
    private bool isFirstUpdate = false;
    public bool IsFirstUpdate {get {return isFirstUpdate;} set {isFirstUpdate = value;}}


    private bool initbool = true;
    
    protected PlayerInputs player;

    public abstract void StartPhase();
    
    public abstract bool EndCheck();

    public abstract void UpdatePhase();

    protected void init(string name){
        this.Name = name;
    }

    protected PlayerInputs getPlayer(){
        if(initbool){
            foreach(NetworkIdentity id in GameMaster.Instance.playerIds){
                if(id.hasAuthority){
                    initbool = false;
                    return id.gameObject.GetComponent<PlayerInputs>();
                }
            }
        }
        return null;
    }
}
