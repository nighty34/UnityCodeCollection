using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPhase : AbstractPhase {

    void Awake(){
        init("Firing");
    }
    // Start is called before the first frame update
    public override void StartPhase(){

        if(player != null){
            player.CanFire = false;
            player.CanMove = false;
            player.CanSelect = false;
            if(IsFirstUpdate || GameMaster.Instance.doNextIteration){
                print("It was done");
                GameMaster.Instance.UnitIteration();
                GameMaster.Instance.doNextIteration = false;
                IsFirstUpdate = false;
            }
        }else{
            this.player = getPlayer();
        }
    }

    public override void UpdatePhase(){
        print(GameMaster.Instance.ActiveUnit);
        //if(player != null && player.hasUnit(GameMaster.Instance.ActiveUnit)){
        if(player != null && GameMaster.Instance.ActiveUnit != null && GameMaster.Instance.ActiveUnit.hasAuthority){
            player.setActiveUnit(GameMaster.Instance.ActiveUnit);
            player.CanFire = true;
            return;
        }else if(player != null){
            player.CanFire = false;
            print("Nicht dein Zug!");
        }
    }

    public override bool EndCheck(){
        print("EndCheck Firing");
        if(player != null){
            return player.Done;
        }
        return false;
    }
}
