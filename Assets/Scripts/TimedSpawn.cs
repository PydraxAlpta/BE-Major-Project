using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawn : MonoBehaviour
{

    public GameObject spawnee;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;
    public int spawnCount = 0;
    private CrowdController crowdController;
    public int maximumSpawns = 25;
    // Use this for initialization
    void Start()
    {
        crowdController = transform.root.GetComponent<CrowdController>();
        crowdController.Pedestrians = new List<GameObject>();
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        if (!spawnee)
        {
            stopSpawning = true;
            CancelInvoke("SpawnObject");
            return;
        }
        var temp = Instantiate(spawnee, transform.position, transform.rotation);
        temp.transform.SetParent(GameObject.FindGameObjectWithTag("Pedestrian").transform,true);
        crowdController.Pedestrians.Add(temp);
        spawnCount++;
        if (spawnCount >= maximumSpawns)
        {
            stopSpawning = true;
            Debug.Log("Spawning Limit reached");
        }
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}