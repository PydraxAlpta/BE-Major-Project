using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    // Start is called before the first frame update
    public long geneticCode;
    public float[] weights;
    public bool runningUnderGA = true;
    public List<GameObject> Pedestrians;
    public int fitness, maxFitness = int.MinValue;
    public GameObject simulation;
    UnityEngine.UI.Text text;
    public int repeatRate;
    public bool finishedSimulation = false;
    void Start()
    {
        fitness = 100;
        text = FindObjectOfType<UnityEngine.UI.Text>();
        var currentSimulation = Instantiate(simulation);
        currentSimulation.transform.SetParent(gameObject.transform);
        if (!runningUnderGA)
            return;
        InterpretGeneticCode();
        text.text = $"Genetic Code: {geneticCode}\nWeights\nPself: {weights[0]}\nVself: {weights[1]}\nDself: {weights[2]}\nPothers: {weights[3]}\nVothers: {weights[4]}\nDothers: {weights[5]}";
        // InvokeRepeating("UpdateSimulation", 0, repeatRate);
    }

    private void InterpretGeneticCode()
    {
        weights = new float[6];
        int[] intWeights = new int[6];
        ulong temp = (ulong)geneticCode;
        const int bitsPerWeight = 64 / 6;
        ulong raisedBits = (ulong)Math.Pow(2,bitsPerWeight);
        for (int i = intWeights.Length - 1; i >= 0; i--)
        {
            intWeights[i] = (int)(temp % raisedBits); //floor division
            temp = temp >> bitsPerWeight; 
        }
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = (float)intWeights[i]/raisedBits;
            weights[i] *=2;
            weights[i] -=1;
        }

    }
    public void UpdateFitness(int change)
    {
        fitness += change;
        if(fitness > maxFitness)
        {
            maxFitness = fitness;
        }
    }
    private bool firstRun = true;
    void FixedUpdate()
    {
        if (firstRun)
        {
            firstRun = false;
        }
        else if (finishedSimulation)
        {
            return;
        }
        else
        {
            CheckSimulationComplete();
            if (finishedSimulation)
            {
                return;
            }
        }
        List<Vector3> Positions = new List<Vector3>(), Velocities = new List<Vector3>(), Destinations = new List<Vector3>();
        Vector3 sumOtherPedestrians = new Vector3();
        foreach (var pedestrian in Pedestrians)
        {

            var rigidbody = pedestrian.GetComponent<Rigidbody>();
            Positions.Add(rigidbody.position);
            Velocities.Add(rigidbody.velocity);
            Destinations.Add(pedestrian.GetComponent<UnityStandardAssets.Characters.ThirdPerson.EthanBehaviour>().destinationVector);

        }
        Vector3 defaultVector = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < Positions.Count; i++)
        {


            sumOtherPedestrians = weights[3] * Positions[i] + weights[4] * Velocities[i] + weights[5] * Destinations[i];

        }
        for (int i = 0; i < Pedestrians.Count; i++)
        {
            Vector3 desiredVector, sumThisPedestrian;
            sumThisPedestrian = (weights[0] * Positions[i] + weights[1] * Velocities[i] + weights[2] * Destinations[i]) - (weights[3] * Positions[i] + weights[4] * Velocities[i] + weights[5] * Destinations[i]);
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
        // Debug.Log("Pedestrians present: " + Pedestrians.Count);
        if (Pedestrians.Count == 0)
        {
            Debug.Log("Simulation finished with fitness " + fitness);
            finishedSimulation = true;
            // Debug.Log(this.gameObject);
            // Destroy(this.gameObject);
        }
    }

    public void EndSimulation()
    {
        Debug.Log("Ending simulation", this.gameObject);
        Destroy(this.gameObject);
    }
}
