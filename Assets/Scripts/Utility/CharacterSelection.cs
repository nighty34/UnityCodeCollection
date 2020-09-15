using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour{


    /*
    ###########################[Public-Vars]###########################
    */
    public Transform tankSpawn;
    public List<Transform> characterSpawns = new List<Transform>();
    public int Tankcounter = 0;
    public List<int> characterCount = new List<int>();

    /*
    ###########################[Private-Vars]###########################
    */
    [SerializeField] private Vector3 characterOffset;
    [SerializeField] private Vector3 tankOffset;
    [SerializeField] private float cameraMoveSpeed = 0.125f;
    private GameObject activeTank;
    private List<GameObject> activeCharacters = new List<GameObject>();
    private int shownCharactersAmount;
    private Object[] tanks;
    private Object[] characters;
    private bool tanksLoaded = false;
    private bool charactersLoaded = false;
    private GameObject activeObject;
    private Camera camera;

    /*
    ###########################[MonoBehavior-Methods]###########################
    */

    void Start(){
        //Load Tanks
        tanks = Resources.LoadAll("Prefab/Tanks", typeof(GameObject));
        if(tanks.Length<=0){
            print("Loading tanks failed! Please make sure that all tanks are in the right folder.");
            tanksLoaded = false;
        }else{
            print("Loaded in: " + tanks.Length + " Tanks");
            tanksLoaded = true;
        }

        //Load Characters
        characters = Resources.LoadAll("Prefab/Characters", typeof(GameObject));
        print("Loaded in: " + characters.Length + "Characters");
        if(characters.Length<=0){
            print("Loading characters failed! Please make sure that all characters are in the right folder.");
            charactersLoaded = false;
        }else{
            charactersLoaded = true;
        }

        fillIDs(characterSpawns.Count);

        camera = GameObject.Find("Camera").GetComponent<Camera>();

        updateTankSpawn();
        updateCharacter(0);
        updateCharacter(1);

        activeObject = activeTank;
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit)){
                print(hit.transform.name);
                if(hit.transform.gameObject.name.Contains("CHARACTER") || hit.transform.gameObject.name.Contains("TANK")){
                    activeObject = hit.transform.gameObject;
                }
            }
        }

        if(activeObject.name.Contains("TANK")){
            MoveCamera(activeObject.transform.position + tankOffset, activeObject.transform);
        }else{
            MoveCamera(activeObject.transform.position + characterOffset, activeObject.transform);
        }
        
    }

    /*
    ###########################[Public-Methods]###########################
    */


    public void nextObject(){
        print(activeObject.name);
        if(activeObject.name.Contains("TANK")){
            nextTank();
        }else if(activeObject.name.Contains("CHARACTER")){
            nextCharacter(int.Parse(activeObject.name.Replace("CHARACTER", "")));
        }
    }

    public void preivousObject(){
            if(activeObject.name.Contains("TANK")){
                previousTank();
            }else if(activeObject.name.Contains("CHARACTER")){
                previousCharacter(int.Parse(activeObject.name.Replace("CHARACTER", "")));
            }
    }
    public void SaveActive(string name){
        PlayerPrefs.SetInt("", Tankcounter); //TODO: SetInputKey
    }

    /*
    ###########################[Private-Methods]###########################
    */
    private void updateTankSpawn(){
        if(tanksLoaded && charactersLoaded){
            if(activeTank != null){
                Destroy(activeTank);
            }
            activeTank = (GameObject)Instantiate(tanks[Tankcounter], tankSpawn.position, tankSpawn.rotation);
            activeTank.name = "TANK";
            activeObject = activeTank;
            activeTank.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void updateCharacter(int characterID){
        if(tanksLoaded && charactersLoaded){
            if(activeCharacters[characterID] != null){
                print(activeCharacters[characterID]);
                Destroy(activeCharacters[characterID]);
            }
            activeCharacters[characterID] = (GameObject)Instantiate(characters[characterCount[characterID]], characterSpawns[characterID].position, characterSpawns[characterID].rotation);
            activeCharacters[characterID].name = "CHARACTER" + characterID;
            activeObject = activeCharacters[characterID];
            activeTank.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void nextTank(){
        Tankcounter++;
        if(Tankcounter>=tanks.Length){
            Tankcounter = 0;
        }
        updateTankSpawn();
    }

    private void previousTank(){
        Tankcounter--;
        if(Tankcounter<0){
            Tankcounter = tanks.Length-1;
            print(Tankcounter);
        }
        updateTankSpawn();
    }

    private void nextCharacter(int i){
        characterCount[i]++;
        if(characterCount[i]>=characters.Length){
            characterCount[i] = 0;
        }
        updateCharacter(i);
    }

    private void previousCharacter(int i){
        characterCount[i]--;
        if(characterCount[i]<0){
            characterCount[i] = characters.Length-1;
        }
        updateCharacter(i);
    }

    private void fillIDs(int count){
        for(int i = 0; i<count; i++){
            characterCount.Add(0);
            activeCharacters.Add(Instantiate(characters[0] as GameObject));
        }
    }

    private void MoveCamera(Vector3 desiredPos, Transform target){
        camera.transform.position = Vector3.Lerp(camera.transform.position, desiredPos, cameraMoveSpeed);
        transform.LookAt(target);
    }
}
