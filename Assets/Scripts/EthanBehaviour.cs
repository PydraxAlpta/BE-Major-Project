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
        public ThirdPersonCharacter character { get; private set; }
        [Range(0.5f,1.0f)]
        public float speed = 0.75f;
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
            character = GetComponent<ThirdPersonCharacter>();
            // speed = UnityEngine.Random.Range(0.5f,1.0f);
            SetDesiredDirection();
        }

        private void SetDesiredDirection()
        {
            desiredDirection = destinationWaypoint.GetPosition() - character.transform.position;
            desiredDirection.y = 0;
            desiredDirection.Normalize();
        }

        void FixedUpdate()
        {
            if (KillBelowYZero())
            {
                return;
            }
            character.Move(speed * desiredDirection, false, false);
            // if (UnityEngine.Random.Range(1, 100) > 98)
            // {
            //     desiredDirection = UnityEngine.Random.insideUnitCircle.normalized;
            //     desiredDirection.z = desiredDirection.y;
            //     desiredDirection.y = 0;
            // }
            if ((character.transform.position - destinationWaypoint.GetPosition()).magnitude < destinationWaypoint.width)
            {
                destinationWaypoint = destinationWaypoint.nextWaypoint;
                if (!destinationWaypoint)
                {
                    Debug.Log("Finished the circuit");
                    KillEthan();
                    return;
                }
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
            TimedSpawn.Pedestrians.Remove(character.gameObject);
            Destroy(character.gameObject);
        }
    }
}
