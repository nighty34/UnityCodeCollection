using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using nightfury;

namespace nightfury
{
    public class PlayerInputs : MonoBehaviour
    {

        [Header("Axis")]
        public string inputSteerAxis = "Horizontal";
        public string inputPowerAxis = "Vertical";
        [Header("Buttons")]
        public string inputFire = "Fire1";
        public string inputInteract = "Fire2";
        public string inputAim = "Fire3";
        public string inputEnter = "Enter";


        public float PowerAxis { get; private set; }
        public float SteerAxis { get; private set; }
        public Boolean interactButton { get; private set; }
        public Boolean fireButton { get; private set; }
        public Boolean aimButton { get; private set; }
        public Boolean enterButton { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            PowerAxis = Input.GetAxis(inputPowerAxis);
            SteerAxis = Input.GetAxis(inputSteerAxis);
            fireButton = Input.GetButton(inputFire);
            interactButton = Input.GetButtonDown(inputInteract);
            aimButton = Input.GetButton(inputAim);
            enterButton = Input.GetButtonDown(inputEnter);
        }
    }
}
