using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawn : MonoBehaviour
{

    public GameObject spawnee;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;
    public static List<GameObject> Pedestrians;

    public int maximumSpawns = 10;
    // Use this for initialization
    void Start()
    {
        Pedestrians = new List<GameObject>();
        Pedestrians.Add(spawnee);
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
        Pedestrians.Add(temp);
        if (GameObject.FindGameObjectsWithTag(spawnee.tag)?.Length >= maximumSpawns)
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