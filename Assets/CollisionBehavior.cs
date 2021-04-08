using UnityEngine;
using UnityEngine.UI;

public class CollisionBehavior : MonoBehaviour
{
    void Start()
    {

    }
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("On Collision Enter: " + collision.collider.name);
    }

    public void OnCollisionStay(Collision collision)
    {
        Debug.Log("On Collision Stay: " + collision.collider.name);
    }

    public void OnCollisionExit(Collision collision)
    {
        Debug.Log("On Collision Exit: " + collision.collider.name);
    }
}