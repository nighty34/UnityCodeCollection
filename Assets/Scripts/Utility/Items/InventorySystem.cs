using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour{
    private Dictionary<PortLocations, Pickupable> inv = new Dictionary<PortLocations, Pickupable>();
    [SerializeField] private Transform InvSpace;

    //activeSlot?
    //PortLocations activeLocation;


    public bool DropSlot(PortLocations location){
        if(inv.ContainsKey(location)){
            inv[location].Drop();
            inv.Remove(location);
            return true;
        }
        return false;
    }


    public bool PickUpSlot(PortLocations location, Pickupable item){
        if(!inv.ContainsKey(location)){
            return Pickup(location, item);
        }
        return false;
    }

    public bool AutoPickSlot(Pickupable item){
        foreach(PortLocations loc in item.GetPortLocations()){
            if(!inv.ContainsKey(loc)){
                return Pickup(loc, item);
            }
        }
        return false;
    }

    public Dictionary<PortLocations, Pickupable> getInv(){
        return inv;
    }


    private bool Pickup(PortLocations location, Pickupable item){
        Transform portLoc = item.PickUp(location);
        if(portLoc !=null){
            inv.Add(location, item);
            item.transform.SetParent(InvSpace);
            return true;
        }
        return false;
    }

}
