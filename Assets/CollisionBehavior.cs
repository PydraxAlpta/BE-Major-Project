using UnityEngine;
using UnityEngine.UI;

public class CollisionBehavior : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("On Collision Enter: " + collision.collider.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("On Collision Stay: " + collision.collider.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("On Collision Exit: " + collision.collider.name);
    }
}