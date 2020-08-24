using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

using Debug = UnityEngine.Debug;


public class TankController : MonoBehaviour
{

    [Header("Generall")]

    public int m_PlayerNumber = 1;
    public GameObject mainObject;
    public GameObject targetedObject;
    public Camera thirdPerson;
    public Camera cannonCamera;
    public Boolean isActive = false; //If is controlled by Player

    [Header("Driving")]
    public float speed;
    public float rotationSpeed;

    public List<WheelCollider> wheelsRight = new List<WheelCollider>();
    public List<WheelCollider> wheelsLeft = new List<WheelCollider>();

    public List<ParticleSystem> engineParticles = new List<ParticleSystem>();

    [Header("Turret")]
    public GameObject turretObject;
    public float turretRotation;
    [Header("Limits")]
    public Boolean limitElevation;
    [Range(0.0f, 180.0f)]
    public float up_limit;
    [Range(0.0f, 180.0f)]
    public float down_limit;
    public Boolean limitRotation;
    [Range(0.0f, 180.0f)]
    public float right_limit;
    [Range(0.0f, 180.0f)]
    public float left_limit;



    [Header("Cannon")]
    public GameObject cannonEnding;
    public GameObject cannonObject;
    public ParticleSystem muzzleflash;

    public GameObject shell;

    public float fireDelta = 0.5F;

    private float nextFire = 0.5F;
    private float myTime = 0.0f;
    private Rigidbody m_Rigidbody;

    private float motorTorque = 100f;



    private float inputFire = 0.5F;
    private float inputTime = 0.0f;




    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        muzzleflash.Stop();
    }


    void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
    }
    // Start is called before the first frame update
    void Start(){
    }

    void Update(){

    }

    void FixedUpdate(){
        if (isActive) { //if is controlled by Player

            myTime = myTime + Time.deltaTime;   //Cannon Cooldown
            inputTime = inputTime + Time.deltaTime; //ChangeView Cooldown
        }

        rotateTurret(targetedObject.transform.position, turretRotation); //Hull rotation


        rotateCannon(targetedObject.transform.position, turretRotation); //Cannon "lift"

        calcDistance(); //TODO: calc velocity to target
    }

    public void Controll(float forwardmovement, float turninput, Boolean interact, Boolean fire, Boolean aim){
        Move(forwardmovement);
        Turn(turninput, forwardmovement);
        ShootCannon(fire);
        ChangeCamera(aim);
        
    }

    private void Move(float movementInputValue){
        if (movementInputValue > 0.001){ //If driving forwards
            foreach (WheelCollider coll in wheelsRight){
                coll.brakeTorque = 0f * Time.deltaTime;
            }
            foreach (WheelCollider coll in wheelsLeft){
                coll.brakeTorque = 0f * Time.deltaTime;
            }

            foreach (WheelCollider coll in wheelsRight){
                coll.motorTorque = movementInputValue * motorTorque * speed * Time.deltaTime;
            }
            foreach (WheelCollider coll in wheelsLeft){
                coll.motorTorque = movementInputValue * motorTorque * speed * Time.deltaTime;
            }
        }else if (movementInputValue < -0.001){ //if driving backwards
            foreach (WheelCollider coll in wheelsRight){
                coll.brakeTorque = 0f * Time.deltaTime;
            }
            foreach (WheelCollider coll in wheelsLeft){
                coll.brakeTorque = 0f * Time.deltaTime;
            }

            foreach (WheelCollider coll in wheelsRight){
                coll.motorTorque = movementInputValue * motorTorque * speed * Time.deltaTime;
            }
            foreach (WheelCollider coll in wheelsLeft){
                coll.motorTorque = movementInputValue * motorTorque * speed * Time.deltaTime;
            }
        }else{  //If noInput
            foreach (WheelCollider coll in wheelsRight){
                coll.brakeTorque = 3000000f * Time.deltaTime;
            }
            foreach (WheelCollider coll in wheelsLeft){
                coll.brakeTorque = 3000000f * Time.deltaTime;
            }
        }
    

         foreach (ParticleSystem part in engineParticles){//DrivingparticleSystem controll

            part.startSpeed = 5 + (movementInputValue * 10);
         }

    }


    private void Turn(float turnInputValue, float movementInputValue){ //Turn Tank (with velocity instead of wheels) TODO: Change to wheelControlled turn
    
        if (movementInputValue >= 0) { 
            float turn = turnInputValue * rotationSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }else{
            float turn = -turnInputValue * rotationSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }
    }

    private void rotateTurret(Vector3 target, float rotationSpeed){ //Rotate Turret
        Vector3 localTargetPos = (target - turretObject.transform.position).normalized;
        localTargetPos.y = 0.0f;

        Vector3 clampedLocalVec2Target = localTargetPos;
        if (limitRotation){
            if (localTargetPos.x >= 0.0f)
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * right_limit, float.MaxValue);
            else
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * left_limit, float.MaxValue);
        }

        Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec2Target);
        Quaternion newRotation = Quaternion.RotateTowards(turretObject.transform.localRotation, rotationGoal, turretRotation * Time.deltaTime);

        turretObject.transform.localRotation = newRotation;
    }

    private void rotateCannon(Vector3 target, float rotationSpeed){ //Elevate Cannon

        Vector3 localTargetPos = (target - cannonObject.transform.position).normalized;
        localTargetPos.x = 0.0f;

        Vector3 clampedLocalVec2Target = localTargetPos;
        if (localTargetPos.y >= 0.0f)
            clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * up_limit, float.MaxValue);
        else
            clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos, Mathf.Deg2Rad * down_limit, float.MaxValue);

        // Create new rotation towards the target in local space.
        Quaternion rotationGoal = Quaternion.LookRotation(clampedLocalVec2Target);
        Quaternion newRotation = Quaternion.RotateTowards(cannonObject.transform.localRotation, rotationGoal, 2.0f * turretRotation * Time.deltaTime);

        // Set the new rotation of the base.
        cannonObject.transform.localRotation = newRotation;
    }

    private void ShootCannon(Boolean shootInput){
        if (shootInput && myTime > nextFire){
            nextFire = myTime + fireDelta;
            GameObject bullet = Instantiate(shell, cannonEnding.transform.position, cannonEnding.transform.rotation);
            muzzleflash.Play();
            bullet.GetComponent<Rigidbody>().AddForce(cannonEnding.transform.forward * 2000000);
            nextFire = nextFire - myTime;
            myTime = 0.0F;
        }

    }

    private void ChangeCamera(Boolean aimInput){
        if (aimInput && inputTime > inputFire){
            inputFire = inputTime + 0.2f;

            if (thirdPerson.enabled){
                thirdPerson.enabled = false;
                cannonCamera.enabled = true;
            }else{
                cannonCamera.enabled = false;
                thirdPerson.enabled = true;
            }
            inputFire = inputFire - inputTime;
            inputTime = 0f;
        }
    }

    public void Enable(){
        thirdPerson.GetComponent<CameraControll>().enabled = true;
        cannonCamera.enabled = false;
        thirdPerson.enabled = true;
        isActive = true;
    }

    public void Disable(){
        isActive = false;
        thirdPerson.GetComponent<CameraControll>().enabled = false;
        cannonCamera.enabled = false;
        thirdPerson.enabled = false;
        
    }


    void calcDistance(){


        /*float distance;
        RaycastHit hit;
        Ray lookingRay = new Ray(cannonEnding.transform.position, Vector3.forward);

        movementInputValue = Input.GetAxis(m_MovementAxisName);
        turnInputValue = Input.GetAxis(m_TurnAxisName);


        /*if (Physics.Raycast(transform.position, transform.forward, out hit)) { 
            Debug.DrawRay(transform.position, transform.forward, Color.green);; 
        }

        //Debug.DrawRay(cannonEnding.transform.position, cannonEnding.transform.forward, Color.green);

        /*if (Physics.Raycast(lookingRay, out hit)){

            Debug.Log("Distance: " + hit.distance);
        }else{
            Debug.Log("Nothing");
        }*/

    }
}
