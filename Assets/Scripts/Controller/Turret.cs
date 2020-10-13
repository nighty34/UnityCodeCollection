using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour{
    [SerializeField] private GameObject cannon;

    [SerializeField] private Transform cannonEnding;
    [SerializeField] private ParticleSystem[] firingParticles;
    [SerializeField] private GameObject shell;
    [SerializeField] private float reloadTime = 0.5f;

    [Header("Turret Rotation")]
    [SerializeField] private bool limitTurretRotation;
    [SerializeField] private float turretSpeed;
    [Range(0.0f, 180.0f)]
    [SerializeField] private float rightLimit;
    [Range(0.0f, 180f)]
    [SerializeField] private float leftLimit;


    [Header("Cannon Elevation")]

    [SerializeField] private bool limitCannonElevation;
    [SerializeField] private float verticalSpeed;
    [Range(0.0f, 180.0f)]
    [SerializeField] private float UpperLimit;
    [Range(0.0f, 180.0f)]
    [SerializeField] private float LowerLimit;

    private float nextFire = 0.5f;
    private float myTime = 0.0f;

    void FixedUpdate(){
        myTime = myTime + Time.deltaTime;
    }


    public void SetTarget(Transform target){
        TurretTurn(target.position);
        GunElevation(target.position);
    }

    private void TurretTurn(Vector3 target){
        Vector3 localTargetPos = (target - transform.position).normalized;
        localTargetPos.y = 0f;

        Vector3 clampedLocalVec = localTargetPos;

        if(limitTurretRotation){
            if(localTargetPos.x >= 0.0f){
                clampedLocalVec = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * rightLimit, float.MaxValue);
            }else{
                clampedLocalVec = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * leftLimit, float.MaxValue);
            }
        }

        Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec);
        Quaternion newRotation = Quaternion.RotateTowards(transform.localRotation, rotationGoal, turretSpeed * Time.deltaTime);

        transform.localRotation = newRotation;     
    }

    private void GunElevation(Vector3 target){
        Vector3 localTargetPos = (target - cannon.transform.position).normalized;
        localTargetPos.x = 0.0f;

        Vector3 clampedLocalVec2Target = localTargetPos;
        if(limitCannonElevation){
            if (localTargetPos.y >= 0.0f){
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * UpperLimit, float.MaxValue);
            }else{
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * LowerLimit, float.MaxValue);
            }  
        }
        Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec2Target);
        Quaternion newRotation = Quaternion.RotateTowards(cannon.transform.localRotation, rotationGoal, verticalSpeed * Time.deltaTime);

        cannon.transform.localRotation = newRotation; 
    }



    public void FireCannon(){
        if (myTime > nextFire){
            nextFire = myTime + reloadTime;
            GameObject bullet = Instantiate(shell, cannonEnding.position, cannonEnding.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(cannonEnding.transform.forward * 20000);
            nextFire = nextFire - myTime;
            myTime = 0.0F;

            foreach(ParticleSystem particle in firingParticles){
                particle.Play();
            }
        }
    }


}
