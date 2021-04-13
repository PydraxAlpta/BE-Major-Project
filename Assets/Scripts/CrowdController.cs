using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    // Start is called before the first frame update
    public string geneticCode;
    public float[] weights;
    public bool runningUnderGA = false;
    public List<GameObject> Pedestrians;
    public int fitness = 100;
    public GameObject simulation;
    void Start()
    {
        // InterpretGeneticCode();
        var currentSimulation = Instantiate(simulation);
        currentSimulation.transform.SetParent(gameObject.transform);
    }

    private void InterpretGeneticCode()
    {
        throw new NotImplementedException();
    }
    public void UpdateFitness(int change)
    {
        fitness+=change;
    }
    public void Simulate()
    {
        throw new NotImplementedException();
    }
}
