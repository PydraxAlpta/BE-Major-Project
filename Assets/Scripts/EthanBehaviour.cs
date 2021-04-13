using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class EthanBehaviour : MonoBehaviour
    {
        public Vector3 desiredDirection;
        public Waypoint destinationWaypoint;
        public Vector3 destinationVector;
        public ThirdPersonCharacter character { get; private set; }
        [Range(0.5f, 1.0f)]
        public float speed = 0.75f;
        private CrowdController crowdController;
        void Start()
        {
            // float x, y = 0f, z;
            // x = UnityEngine.Random.Range(-1.0f, 1.0f);
            // z = UnityEngine.Random.Range(-1.0f, 1.0f);
            // randomDirection = new Vector3(x, y, z);
            // randomDirection = randomDirection.normalized;
            // desiredDirection = UnityEngine.Random.insideUnitCircle.normalized;
            // desiredDirection.z = desiredDirection.y;
            // desiredDirection.y = 0;
            crowdController = transform.root.GetComponent<CrowdController>();
            destinationWaypoint = Waypoints.GetFirstWaypoint();
            character = GetComponent<ThirdPersonCharacter>();
            // speed = UnityEngine.Random.Range(0.5f,1.0f);
            SetDesiredDirection();
        }

        private void SetDesiredDirection()
        {
            if (crowdController.runningUnderGA)
                return;
            destinationVector = destinationWaypoint.GetPosition();
            desiredDirection = destinationVector - character.transform.position;
            desiredDirection.y = 0;
            desiredDirection.Normalize();
        }
        public void SetDesiredDirection(Vector3 vector)
        {
            if(!crowdController.runningUnderGA)
                return;
            desiredDirection = vector;
        }
        void FixedUpdate()
        {
            if (KillBelowYZero())
            {
                return;
            }
            if (UnityEngine.Random.Range(1, 1000) > 990)
                SetDesiredDirection(); //small probabilistic call to set on the right direction 
                                       //if it misses the waypoint for whatever reason
            character.Move(speed * desiredDirection, false, false);
            // if (UnityEngine.Random.Range(1, 100) > 98)
            // {
            //     desiredDirection = UnityEngine.Random.insideUnitCircle.normalized;
            //     desiredDirection.z = desiredDirection.y;
            //     desiredDirection.y = 0;
            // }
            if ((character.transform.position - destinationVector).magnitude <= destinationWaypoint.width)
            {
                destinationWaypoint = destinationWaypoint.nextWaypoint;
                if (!destinationWaypoint)
                {
                    Debug.Log("Finished the circuit");
                    crowdController.UpdateFitness(100);
                    KillEthan();
                    return;
                }
                crowdController.UpdateFitness(5);
                SetDesiredDirection();
            }
        }
        public bool KillBelowYZero()
        {
            if (character.transform.position.y < 0)
            {
                Debug.Log("Despawned from falling off the map ", character);
                KillEthan();
                return true;
            }
            return false;
        }

        private void KillEthan()
        {
            crowdController.Pedestrians.Remove(character.gameObject);
            Destroy(character.gameObject);
        }
    }
}
