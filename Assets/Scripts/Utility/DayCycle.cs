using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayCycle : MonoBehaviour{

    private static DayCycle instance;
    public static DayCycle Instance{
        get{return Instance;}
    }

    public float speed = 30f;

    public static UnityEvent MorningEvent;
    public static UnityEvent EveningEvent;

    private float time; 


    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this);
        }else{
            Destroy(this);
        }
    }

    void Update(){
        transform.RotateAround(Vector3.zero, Vector3.right, speed*Time.deltaTime);
        transform.LookAt(Vector3.zero);
        switch(time){
            case 600:
                MorningEvent.Invoke();
            break;

            case 20000:
                EveningEvent.Invoke();
            break;
        }
    }
}
