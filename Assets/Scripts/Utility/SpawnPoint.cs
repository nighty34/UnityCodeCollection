using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class SpawnPoint : MonoBehaviour{
        [SerializeField] int team = 0;
        private void Awake(){
            switch(team){
                case 0:
                    SpawnSystem.AddSpawnPointA(transform);
                    break;
                case 1:
                    SpawnSystem.AddSpawnPointB(transform);
                    break;
            }

        }
        private void OnDestroy(){
            switch(team){
                case 0:
                    SpawnSystem.RemoveSpawnPointA(transform);
                    break;
                case 1:
                    SpawnSystem.RemoveSpawnPointB(transform);
                    break;
            }
            SpawnSystem.RemoveSpawnPointA(transform);
        }

        private void OnDrawGizmos(){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
        }
    }
