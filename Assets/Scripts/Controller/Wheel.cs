using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour{
    [SerializeField] private bool steerable = false;
    [SerializeField] private bool powerable = false;
    [SerializeField] private bool inverted = false;

    [SerializeField] private Transform wheelTransform;
    [SerializeField] private WheelCollider wheelCollider;


    void FixedUpdate(){
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
  

        if(wheelCollider!=null && wheelTransform!=null){
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.position = pos;
            if(inverted){
                wheelTransform.rotation = rot * Quaternion.Euler(0, 180, 0);
            }else{
                wheelTransform.rotation = rot * Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void ApplyForces(float steerInput, float torqueInput, float breakingInput){
        wheelCollider.brakeTorque = breakingInput;
        if(steerable){
            float newSteerAngle = Mathf.LerpAngle(wheelCollider.steerAngle, steerInput, 0.1f);
            wheelCollider.steerAngle = newSteerAngle;
        }
        if(powerable){
            wheelCollider.motorTorque = torqueInput;
        }
    }

    public bool Equals(Wheel other){
        return other.wheelTransform == this.wheelTransform;
    }
}
