using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortLocationComponent : MonoBehaviour{
    public Item item;
    public PortLocations loc;

    void Awake(){
        item.AddLocation(loc, this.transform);
    }
}
