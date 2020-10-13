using UnityEngine;

public class CarController : Rideable{

    [SerializeField] private Wheel[] wheels;
    [SerializeField] private float motorTorque = 2000f;
    [SerializeField] private float steeringAngle = 60f;
    [SerializeField] private float breakingTorque = 2000f;
    [Tooltip("Speed in Km/h")]
    [SerializeField] private float topSpeed = 80f;

    public override void Controll(InputMaster input){
        Vector2 inputs = input.Cardriving.Steering.ReadValue<Vector2>();

        float powerInput = 0;
        float breakInput = 0;


        if(getForwardVelocity()>0.01 && inputs.y < 0){
            breakInput = inputs.y * -1;
        }else{
            powerInput = inputs.y;
        }
        

        foreach(Wheel wheel in wheels){
            wheel.ApplyForces(inputs.x * steeringAngle, powerInput * motorTorque, breakInput * breakingTorque);
        }
    }

    void Update(){
        CapSpeed();
    }
    
    protected override void whenEnterAsDriver(){
    }

    protected override void whenEnterAsPassanger(){
    }

    protected override void whenLeave(){
        foreach(Wheel wheel in wheels){
            wheel.ApplyForces(0, 0, 200  * breakingTorque);
        }
    }


    protected override void setUpCamera(){
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
