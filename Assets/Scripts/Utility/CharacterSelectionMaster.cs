using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionMaster : MonoBehaviour
{
    private CharacterSelection cs;
    void Start(){
        cs = GameObject.Find("CharacterSelection").GetComponent<CharacterSelection>();
    }


    public void IntoBattle(){
        PlayerPrefs.SetInt("playerTankID", cs.Tankcounter);

        //save Characters
        
        //Load Scene
    }
}
