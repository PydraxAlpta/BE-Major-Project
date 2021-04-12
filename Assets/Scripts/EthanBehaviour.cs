using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class EthanBehaviour : MonoBehaviour
    {
        public Vector3 randomDirection;
        public ThirdPersonCharacter character { get; private set; }
        void Start()
        {
            // float x, y = 0f, z;
            // x = UnityEngine.Random.Range(-1.0f, 1.0f);
            // z = UnityEngine.Random.Range(-1.0f, 1.0f);
            // randomDirection = new Vector3(x, y, z);
            // randomDirection = randomDirection.normalized;
            randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            randomDirection.z = randomDirection.y;
            randomDirection.y = 0;
            character = GetComponent<ThirdPersonCharacter>();
        }
        void FixedUpdate()
        {
            character.Move(randomDirection, false, false);
            if (UnityEngine.Random.Range(1, 100) > 98)
            {
                randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                randomDirection.z = randomDirection.y;
                randomDirection.y = 0;
            }
            if (character.transform.position.y < 0)
            {
                Debug.Log("Despawned from falling off the map ",character);
                TimedSpawn.Pedestrians.Remove(character.gameObject);
                Destroy(character.gameObject);
            }
        }
    }
}
