using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelTransform;
    void FixedUpdate(){
        var pos = Vector3.zero;
        var rot = Quaternion.identity;

        if(wheelCollider!=null && wheelTransform!=null){
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.position = pos;
            wheelTransform.rotation = rot * Quaternion.Euler(0, 180, 90);
        }
    }

    public Wheel(WheelCollider wheelCollider, Transform wheelTransform){
        this.wheelCollider = wheelCollider;
        this.wheelTransform = wheelTransform;
    }

    public bool Equals(Wheel other){
        return other.wheelTransform == this.wheelTransform;
    }
}
