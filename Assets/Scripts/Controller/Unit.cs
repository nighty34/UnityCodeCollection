using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;


[RequireComponent(typeof(TankPathFinder))]
public class Unit : NetworkBehaviour{

    private static string MUZZLEFLASHNAME = "muzzleflash";

    public bool isActive = false;
    [SyncVar]
    public Tank stats = null;
    public bool hasMoved = false;
    public float distance = 4.5f;
    public Vector3 desitnation;
    public PlayerInputs playerInputs;
    public TankObject model;
    private Transform turret;
    private Transform aimedAt;
    private bool modellLoaded = false;
    [SerializeField] private TankPathFinder navigator = null;
    public TankPathFinder Navigator{get{return navigator;} private set {navigator = value;}}

    private static string TANKFOLDER = "tanks/";

    private void FindPlayerInputs(){
        foreach(PlayerInputs inputs in GameMaster.Instance.players){
            if(inputs.hasAuthority){
                playerInputs = inputs;
            }
        }
    }


    void Start(){
        navigator = GetComponent<TankPathFinder>();
        FindPlayerInputs();
    }
    
    public override void OnStartAuthority(){
        //GameMaster.Instance.addUnit(this.netIdentity);
        Cmd_AddToGameMaster(this.netIdentity);
    }

    void Update(){
        if(aimedAt != null){ 
            Quaternion rotation = Quaternion.LookRotation(aimedAt.position - turret.position);
            turret.rotation = Quaternion.Slerp(turret.rotation, rotation, Time.deltaTime * stats.turretRotation);
        }
        if(!modellLoaded){
            GameObject tankModel = (GameObject) Resources.Load(TANKFOLDER + stats.modelname);
            GameObject newModel = Instantiate(tankModel, transform.position, transform.rotation);
            newModel.transform.SetParent(transform);
            model = newModel.GetComponent<TankObject>();
            modellLoaded = true;
        }
    }

    public void setTarget(Vector3 newDest){
        Cmd_SetTarget(newDest);
    }


    private Vector3 updateTarget(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    
        Vector3 point = ray.origin + (ray.direction * distance);  
        desitnation = point;  
        return point;
    }

    public void AimAt(Transform target){
        Cmd_AimAt(target);
    }

    public void Fire(){
        Cmd_Fire();
    }

    public void Hit(){
        //HITS?
        //DESTROYS?
        //REMOVE FROM ACTIVE UNITS REMOVE FROM FIREINITATIVE
    }

    private bool calcKill(){
        return false;
    }


    /*########################
    Command
    ########################*/

    [Command]
    private void Cmd_Fire(){
        Rpc_Fire();
        if(calcKill()){
            aimedAt.GetComponent<Unit>().Rpc_Kill();
        }
    }

    [Command]
    private void Cmd_AimAt(Transform target){
        //VALIDATE
        Rpc_AimAt(target);
    }


    [Command]
    private void Cmd_SetTarget(Vector3 newTarget){
        //VALIDATE
        Rpc_SetTarget(newTarget);
    }

    [Command]
    private void Cmd_AddToGameMaster(NetworkIdentity id){
        GameMaster.Instance.Rpc_addUnit(id);
    }



    /*########################
    ClientRpc
    ########################*/
    [ClientRpc]
    private void Rpc_Kill(){
        GameMaster.Instance.removeUnit(this.netIdentity);
        Destroy(this);
    }



    [ClientRpc]
    private void Rpc_Fire(){
        GameObject cannonEnding = model.TankComponents["CannonEnding"].gameObject;
        GameObject muzzleflash = Resources.Load(MUZZLEFLASHNAME) as GameObject;
        print(muzzleflash);
        Instantiate(muzzleflash, cannonEnding.transform).GetComponent<ParticleSystem>().Play();
    }


    [ClientRpc]
    private void Rpc_AimAt(Transform target){
        turret = model.TankComponents["Turret"];
        aimedAt = target;
    }

    [ClientRpc]
    private void Rpc_SetTarget(Vector3 newTarget){
        navigator.moveToDestination(newTarget);
    }
}
