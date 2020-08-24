using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour{


    private bool isPickedUp = false;    
    private Dictionary<PortLocations, Transform> rootLocations = new Dictionary<PortLocations, Transform>();
    private Rigidbody m_Rigidbody;


    void Start(){
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update(){
        //Sync Location;
    }


    public Transform PickUp(PortLocations loc){
        if(!isPickedUp){
            isPickedUp = true;
            m_Rigidbody.isKinematic = true;
            //SET COLLIDER FALSE;
            return rootLocations[loc];
        }
        return null;
    }

    public void Drop(){
        isPickedUp = false;
        m_Rigidbody.isKinematic = false;
        print(GameMaster.Instance.ItemDropSpace);
        transform.SetParent(GameMaster.Instance.ItemDropSpace.transform);
    }

    public List<PortLocations> GetPortLocations(){
        List<PortLocations> locs = new List<PortLocations>();
        foreach(PortLocations loc in rootLocations.Keys){
            locs.Add(loc);
        }
        return locs;
    }

    private Vector3 getOffset(PortLocations loc){
        return this.transform.position - rootLocations[loc].position;
    }

    public void AddLocation(PortLocations loc, Transform transform){
        rootLocations.Add(loc, transform);
    }
}
