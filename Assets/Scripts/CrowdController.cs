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
    public int fitness;
    public GameObject simulation;
    public UnityEngine.UI.Text text;
    public int repeatRate;
    void Start()
    {
        fitness = 100;
        var currentSimulation = Instantiate(simulation);
        currentSimulation.transform.SetParent(gameObject.transform);
        if (!runningUnderGA)
            return;
        InterpretGeneticCode();
        text.text = $"Genetic Code: {geneticCode.ToString()}\nWeights\nPself: {weights[0]}\nVself: {weights[1]}\nDself: {weights[2]}\nPothers: {weights[3]}\nVothers: {weights[4]}\nDothers: {weights[5]}";
        InvokeRepeating("UpdateSimulation", 0, repeatRate);
    }

    private void InterpretGeneticCode()
    {
        weights = new float[6];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = UnityEngine.Random.Range(-1.0f, 1.0f);
        }
    }
    public void UpdateFitness(int change)
    {
        fitness += change;
    }
    private bool firstRun = true;
    void UpdateSimulation()
    {
        if(firstRun)
        {
            firstRun = false;
        }
        else
        {
            CheckSimulationComplete();
        }
        List<Vector3?> Positions = new List<Vector3?>(), Velocities = new List<Vector3?>(), Destinations = new List<Vector3?>();
        Vector3 sumOtherPedestrians = new Vector3();
        foreach (var pedestrian in Pedestrians)
        {
            if (pedestrian != null)
            {
                var rigidbody = pedestrian.GetComponent<Rigidbody>();
                Positions.Add(rigidbody.position);
                Velocities.Add(rigidbody.velocity);
                Destinations.Add(pedestrian.GetComponent<UnityStandardAssets.Characters.ThirdPerson.EthanBehaviour>().destinationVector);
            }
            else
            {
                Positions.Add(null);
            }
        }
        Vector3 defaultVector = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < Positions.Count; i++)
        {

            if (Positions[i] != null)
            {
                sumOtherPedestrians = weights[3] * Positions[i] ?? defaultVector + weights[4] * Velocities[i] ?? defaultVector + weights[5] * Destinations[i] ?? defaultVector;
            }
        }
        for (int i = 0; i < Pedestrians.Count; i++)
        {
            Vector3 desiredVector, sumThisPedestrian;
            sumThisPedestrian = (weights[0] * Positions[i] ?? defaultVector + weights[1] * Velocities[i] ?? defaultVector + weights[2] * Destinations[i] ?? defaultVector) - (weights[3] * Positions[i] ?? defaultVector + weights[4] * Velocities[i] ?? defaultVector + weights[5] * Destinations[i] ?? defaultVector);
            desiredVector = sumThisPedestrian + sumOtherPedestrians;
            if (desiredVector.magnitude > 1)
            {
                desiredVector.Normalize();
            }
            Pedestrians[i].GetComponent<UnityStandardAssets.Characters.ThirdPerson.EthanBehaviour>().SetDesiredDirection(desiredVector);
        }
    }

    public void CheckSimulationComplete()
    {
        if (Pedestrians.Count == 0)
        {
            Destroy(this);
        }
        else
        {
            Debug.Log("Pedestrians present: " + Pedestrians.Count);
        }
    }
}
