using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FiringMode{
    LEVERACTION,
    PUMPACTION,
    SEMIAUTO,
    AUTO,
    MANUAL
}

public enum AmmonitionType{
    CLIP,
    SINGLE,
    MAGAZINE
}

[CreateAssetMenu(fileName = "FireArm", menuName = "ScriptableObjects/Weapons/FireArm")]
public class FiringWeapon : ScriptableObject{
    [SerializeField]
    private float firingSpeed;
    [SerializeField]
    private FiringMode firingMode;
    [SerializeField]
    private float caliber; //mm
    [SerializeField]
    private AmmonitionType ammonitionType;

    public float FiringSpeed {private set{} get {return firingSpeed;}}
    public FiringMode FiringMode {private set{} get {return firingMode;}}
    public float Caliber {private set{} get {return caliber;}}
    public AmmonitionType AmmonitionType {private set{} get {return ammonitionType;}}
}
