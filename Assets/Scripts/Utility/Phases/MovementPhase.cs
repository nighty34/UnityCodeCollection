using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPhase : AbstractPhase
{
    void Awake(){
        init("Movement");
    }
    // Start is called before the first frame update
    public override void StartPhase(){
        if(player != null){
            player.CanFire = false;
            player.CanMove = true;
            player.CanSelect = true;
        }else{
            this.player = getPlayer();
        }
    }

    public override void UpdatePhase(){
        print("UPDATE MOVEMENT");
    }

    public override bool EndCheck(){
        print("EndCheck Movement");
        if(player != null){
            return player.Done;
        }
        return false;
    }





}
