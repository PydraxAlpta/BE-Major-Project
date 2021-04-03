using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthanCollisionBehaviour : MonoBehaviour
{
    void onTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
