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
        // Debug.Log("On Collision Enter: " + collision.collider.name);
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag != "Walkable")
        {
            Debug.Log("Collided with non walkway",this.gameObject);
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        // Debug.Log("On Collision Stay: " + collision.collider.name);
        if(collision.gameObject.tag != "Walkable")
        {
            Debug.Log("Colliding with non walkway",this.gameObject);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        Debug.Log("On Collision Exit: " + collision.collider.name);
    }
}