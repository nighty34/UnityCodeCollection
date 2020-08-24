using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UIHandler : NetworkBehaviour{
    [SerializeField] private TMP_Text activePhaseText;
    public PlayerInputs player;

    void Start(){
        //Draw TankIcons
    }

    // Update is called once per frame
    void Update(){

        activePhaseText.text = player.GetPhase().Name;
    }
}
