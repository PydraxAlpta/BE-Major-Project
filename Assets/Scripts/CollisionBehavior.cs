using UnityEngine;
using UnityEngine.UI;

public class CollisionBehavior : MonoBehaviour
{
    private CrowdController crowdController;
    void Start()
    {
        crowdController = transform.root.GetComponent<CrowdController>();
    }
    void Update()
    {

    }
    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("On Collision Enter: " + collision.collider.name);
        // Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag != "Walkable")
        {
            // Debug.Log("Collided with non walkway",this.gameObject);
            crowdController.UpdateFitness(-100);
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        // Debug.Log("On Collision Stay: " + collision.collider.name);
        if (collision.gameObject.tag != "Walkable")
        {
            // Debug.Log("Colliding with non walkway",this.gameObject);
            if (UnityEngine.Random.Range(0, 10) > 5)
                crowdController.UpdateFitness(-1);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        // Debug.Log("On Collision Exit: " + collision.collider.name);
        if (collision.collider.tag != "Walkable")
        {
            crowdController.UpdateFitness(50);
        }
    }
}