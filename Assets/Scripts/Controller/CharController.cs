using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


class CameraState{
    public float yaw;
    public float pitch;
    public float roll;
    public float x;
    public float y;
    public float z;

    public void SetFromTransform(Transform t){
        pitch = t.eulerAngles.x;
        yaw = t.eulerAngles.y;
        roll = t.eulerAngles.z;
        x = t.position.x;
        y = t.position.y;
        z = t.position.z;
    }

    public void Translate(Vector3 translation){
        Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

        x += rotatedTranslation.x;
        y += rotatedTranslation.y;
        z += rotatedTranslation.z;
    }

    public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct){
        yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
        pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
        roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                
        x = Mathf.Lerp(x, target.x, positionLerpPct);
        y = Mathf.Lerp(y, target.y, positionLerpPct);
        z = Mathf.Lerp(z, target.z, positionLerpPct);
    }

    public void UpdateTransform(Transform t){
        t.eulerAngles = new Vector3(pitch, yaw, roll);
        t.position = new Vector3(x, y, z);
    }
}
[RequireComponent(typeof(InventorySystem), typeof(CameraActions))]
public class CharController : MonoBehaviour{

    /*
    ###########################[Public-Vars]###########################
    */


    public GameObject mainframe;
    public float movementSpeed = 0f;
    public float turnSmoothTime = 0.1f;
    public PlayerInputs inputs {set; private get;}

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform m_camera;


    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
    public bool invertY = false;
    public float positionLerpTime = 0.2f;
    public float yTarget;
    public BaseLayerAniCon aniCon;

    /*
    ###########################[Private-Vars]###########################
    */


    private CharacterController characterController;
    private Boolean isGrounded;
    private Vector3 velocity;
    private CameraState m_TargetCameraState = new CameraState();
    private float TurnSmoothVelocity;

    private InputMaster inputMaster;
    private CameraActions cameraActions;
    private InventorySystem inventory;


    /*
    ###########################[MonoBehavior-Methods]###########################
    */

    // Start is called before the first frame update
    void Awake(){
        characterController = GetComponent<CharacterController>();
        cameraActions = GetComponent<CameraActions>();
        inventory = GetComponent<InventorySystem>();

        inputMaster = new InputMaster();

        inputMaster.Player.Interact.performed += ctx => PickUp();
        inputMaster.Player.DropAll.performed += ctx => DropAll();
    }

    void OnEnable(){
        m_TargetCameraState.SetFromTransform(transform);
        inputMaster.Enable();
    }

    void OnDisabe(){
        inputMaster.Disable();
    }

    void Start(){
        GameMaster.Instance.registerCharController(this);
        aniCon.Start();
    }

    // Update is called once per frame
    void Update(){

        aniCon.Play(1);

        if (Input.GetMouseButtonDown(1)){
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetMouseButtonUp(1)){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
    }

    void FixedUpdate() { 
        Movement();
        velocity += Physics.gravity * Time.deltaTime;
    }



    /*
    ###########################[Public-Methods]###########################
    */


    /* 
    *  External Controll 
    */
    /*public void Controll(float forwardmovement, float turninput, Boolean interact, Boolean fire, Boolean enter){
        Movement(turninput, forwardmovement);
        //rotateView();
    }*/


    /*
    ###########################[Private-Methods]###########################
    */


    /*
    * Controll via PlayerInput
    */
    /*private void Controll(){
        Movement(inputs.SteerAxis, inputs.PowerAxis);
    }*/

    private void Movement(){
        Movement(inputMaster.Player.Movement.ReadValue<Vector2>());
    }

    private void Movement(Vector2 inp){
        Vector3 direction = new Vector3(inp.x, 0f, inp.y).normalized;

        if(direction.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * movementSpeed * Time.deltaTime);
        }


        characterController.Move(velocity * Time.deltaTime);

    }

    private void Rotate(float turnInputValue, float movementInputValue){
        if (movementInputValue >= 0){
            float turn = turnInputValue * movementSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        }else{
            float turn = -turnInputValue * movementSpeed  * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        }
    }

    private void Rotate(){
        float horizontal = 1f;
        float vertical = 1f;
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if(direction.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + m_camera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


        }
    }

    private void rotateView(){
        if (Input.GetMouseButton(1)){
            //float rotation = 0f;
            var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
            
            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            //characterController.transform.rotation.eulerAngles.y += rotation;
        }
    }

    private void PickUp(){
        GameObject obj = cameraActions.GetFacingItem();
        if(obj != null){
            Pickupable item = obj.GetComponent<Pickupable>();
            if(inventory.AutoPickSlot(item)){
                print("Item was picked Up");
                return;
            }else{
                print("Missing InventorySpace");
            }
        }else{
            print("No Item was found");
        }
    }

    private void DropAll(){
        inventory.DropSlot(PortLocations.BACK);
        inventory.DropSlot(PortLocations.BELT);
        inventory.DropSlot(PortLocations.LEFT_HAND);
        inventory.DropSlot(PortLocations.POCKET);
        inventory.DropSlot(PortLocations.RIGHT_HAND);
    }
}


