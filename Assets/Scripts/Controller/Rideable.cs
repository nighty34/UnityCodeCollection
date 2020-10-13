using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Rideable : Interactable{

    [SerializeField] protected CinemachineFreeLook lookCam;
    protected Rigidbody rb = null;
    protected InputMaster input;

    void Awake(){
        rb = this.GetComponent<Rigidbody>();
    } 

    public override void Interact(CharController actor){
        this.input = actor.InputMaster;
        actor.EnterVehicle(this);
    }

    protected CharController driver;
    protected List<CharController> passangers = new List<CharController>();
    [SerializeField] private int maxPassanger = 0;

    protected abstract void whenEnterAsDriver();
    protected abstract void whenEnterAsPassanger();
    protected abstract void whenLeave();
    
    
    public bool EnterAsDriver(CharController character){
        lookCam.Priority = 15;
        if(driver==null){
            driver = character;
            whenEnterAsDriver();
            return true;
        }
        return false;
    }

    public bool EnterAsPassanger(CharController character){
        lookCam.Priority = 15;
        if(!(passangers.Count>=maxPassanger)){
            passangers.Add(character);
            whenEnterAsPassanger();
            return true;
        }
        return false;
    }

    public bool EnterAuto(CharController character){
        if(EnterAsDriver(character)){
            return true;
        }
        if(EnterAsPassanger(character)){
            return true;
        }
        return false;
    }

    public void LeaveVehicle(CharController character){
        lookCam.Priority = 5;
        whenLeave();
        if(character == driver){
            driver = null;
            return;
        }
        if(passangers.Contains(character)){
            passangers.Remove(character);
            return;
        }
    }

    public abstract void Controll(InputMaster inputs);

    protected abstract void setUpCamera();
}
