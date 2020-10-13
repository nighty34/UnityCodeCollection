using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : Rideable{

    [SerializeField] private Turret[] turrets;
    [SerializeField] private Transform target;
    [SerializeField] private TrackRolers[] trackRolersRight;
    [SerializeField] private TrackRolers[] trackRolersLeft;

    [SerializeField] private float motorTorque = 2000f;
    [SerializeField] private float breakingTorque = 2000f;

    [Tooltip("Speed in Km/h")]
    [SerializeField] private float topSpeed = 50f;
    public Vector3 aimOffset;



    private bool isAiming = false;

    public override void Controll(InputMaster input){
        Vector2 inputs = input.Cardriving.Steering.ReadValue<Vector2>();

        float powerInput = 0;
        float breakInput = 0;


        if(getForwardVelocity()>0.01 && inputs.y < 0){
            breakInput = inputs.y * -1;
        }else{
            powerInput = inputs.y;
        }

        float leftPower;
        float rightPower;

        if(inputs.x>0.1 || inputs.x<-0.1){
        
            float steeringAngle = inputs.x;
            rightPower = Mathf.Clamp(powerInput * steeringAngle  *-1, -1, 1);
            leftPower = Mathf.Clamp(powerInput * steeringAngle, -1, 1);
        }else{
            leftPower = powerInput;
            rightPower = powerInput;
        }

        foreach(TrackRolers trackRoler in trackRolersRight){
            trackRoler.ApplyForces(rightPower * motorTorque, breakInput * breakInput);
        }

        foreach(TrackRolers trackRoler in trackRolersLeft){
            trackRoler.ApplyForces(leftPower * motorTorque, breakInput * breakInput);
        }
    }

    
    void Update(){
        Ray ray = GameMaster.Instance.mainCamera.ViewportPointToRay(aimOffset);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)){
            target.position = hit.point;
        }
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 0.1f, false);

        foreach(Turret turret in turrets){
            if(isAiming){
                turret.SetTarget(target);
            }

            if(input!=null){
            input.Player.FiringWeapon.performed += ctx => turret.FireCannon();
            }
        }
        CapSpeed();
    }

    protected override void setUpCamera(){
    }

    protected override void whenEnterAsDriver(){
        isAiming = true;
    }

    protected override void whenEnterAsPassanger(){
        isAiming = true;
    }

    protected override void whenLeave(){
        isAiming = false;
    }

        private float getForwardVelocity(){
        return transform.InverseTransformDirection(rb.velocity).z;
    }

    private void CapSpeed(){
        float speed = rb.velocity.magnitude;
        speed *= 3.6f;
        if(speed>topSpeed){
            rb.velocity = (topSpeed/3.6f) * rb.velocity.normalized;
        }
    }

}
