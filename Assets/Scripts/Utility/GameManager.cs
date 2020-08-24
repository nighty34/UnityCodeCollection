using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nightfury;

namespace nightfury
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public List<PlayerInputs> playerInput = new List<PlayerInputs>();

        void Awake()
        {
            Instance = this;
            playerInput.Add(GetComponentInChildren<PlayerInputs>());
        }

        void Update()
        {

        }

    }
}

