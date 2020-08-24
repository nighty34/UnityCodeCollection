using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tank", menuName = "Tanks/Tank")]
public class Tank : ScriptableObject
{
    public new string name;
    public TankTypeEnum tankClass;
    public string description;
    //public TankObject model;
    public string modelname;
    public int cost;
    public float tavelDistance;
    
    public float turretRotation;
    //armor is in TankObject



}
