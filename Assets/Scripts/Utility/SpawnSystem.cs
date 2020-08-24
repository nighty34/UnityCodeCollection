using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnSystem : NetworkBehaviour{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Tank defaultTank = null;
    [SerializeField] private GameObject playerInputPrefab = null;
    

    private static List<Transform> spawnA = new List<Transform>();
    private static List<Transform> spawnB = new List<Transform>();

    private static string TANKFOLDER = "tanks/";

    private int nextAIndex = 0;
    private int nextBIndex = 0;

    /*public void SpawnTeamA(Tank tank, PlayerInputs inputs){
        GameObject actObj = Instantiate(unitPrefab, spawnA[0]);
        Unit actUnit = actObj.GetComponent<Unit>();
        actUnit.stats = tank;
        GameObject tankModel = (GameObject) Resources.Load(TANKFOLDER + actUnit.stats.modelname);
        GameObject newModel = Instantiate(tankModel, actObj.transform.position, actObj.transform.rotation);
        newModel.transform.SetParent(actObj.transform);
        actUnit.model = newModel.GetComponent<TankObject>();
        actUnit.playerInputs = inputs;
        inputs.addUnit(actUnit);
        //Modell aus dem Tank raus nehmen und stattdessen nen String hinzufügen, der auf den Prefab verweist. Diese dann Instanziieren 
    }

    public void SpawnTeamB(Tank tank, PlayerInputs inputs){
        GameObject actObj = Instantiate(unitPrefab, spawnB[0]);
        Unit actUnit = actObj.GetComponent<Unit>();
        actUnit.stats = tank;
        GameObject tankModel = (GameObject) Resources.Load(TANKFOLDER + actUnit.stats.modelname);
        GameObject newModel = Instantiate(tankModel, actObj.transform.position, actObj.transform.rotation);
        newModel.transform.SetParent(actObj.transform);
        actUnit.model = newModel.GetComponent<TankObject>();
        actUnit.playerInputs = inputs;
        inputs.addUnit(actUnit);
    }*/

    public override void OnStartServer() => NetworkManagerRTS.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerRTS.OnServerReadied -= SpawnPlayer;

    public static void AddSpawnPointA(Transform transform){
        spawnA.Add(transform);
    }

    public static void AddSpawnPointB(Transform transform){
        spawnB.Add(transform);
    }

    public static void RemoveSpawnPointA(Transform transform) => spawnA.Remove(transform); 

    public static void RemoveSpawnPointB(Transform transform) => spawnB.Remove(transform);

    [Server]
    public void SpawnPlayer(NetworkConnection conn){
        if(true){ //test for team
            SpawnPlayerInputs(conn);
            Transform spawnPoint = spawnA[nextAIndex];
            if(spawnPoint == null){
                Debug.LogError($"Missing spawn point for player {nextAIndex}");
                return;
            }
    
            GameObject playerInstance = Instantiate(unitPrefab, spawnA[nextAIndex].position, spawnA[nextAIndex].rotation);
            setModell(playerInstance, defaultTank);
            NetworkServer.Spawn(playerInstance, conn);

            nextAIndex++;
        }
    }

    
    [Server]
    private void addModell(GameObject actObj, PlayerInputs inputs, Tank tank){
        Unit actUnit = actObj.GetComponent<Unit>();
        actUnit.stats = tank;
        GameObject tankModel = (GameObject) Resources.Load(TANKFOLDER + actUnit.stats.modelname);
        GameObject newModel = Instantiate(tankModel, actObj.transform.position, actObj.transform.rotation);
        newModel.transform.SetParent(actObj.transform);
        actUnit.model = newModel.GetComponent<TankObject>();
        actUnit.playerInputs = inputs;
        //inputs.addUnit(actUnit);
    }

    [Server]
    private void setModell(GameObject tankObj, Tank tank){
        tankObj.GetComponent<Unit>().stats = tank;
    }

    [ClientRpc]
    private void Rpc_AddModell(GameObject actObj, Tank tank){
        Unit actUnit = actObj.GetComponent<Unit>();
        print(actUnit);
        actUnit.stats = tank;
        GameObject tankModel = (GameObject) Resources.Load(TANKFOLDER + actUnit.stats.modelname);
        GameObject newModel = Instantiate(tankModel, actObj.transform.position, actObj.transform.rotation);
        newModel.transform.SetParent(actObj.transform);
        actUnit.model = newModel.GetComponent<TankObject>();
        //actUnit.playerInputs = inputs;
    }

    [Server]
    private void SpawnPlayerInputs(NetworkConnection conn){
        GameObject newInputs = Instantiate(playerInputPrefab);
        NetworkServer.Spawn(newInputs, conn);
        print("spawned");
    }
}
