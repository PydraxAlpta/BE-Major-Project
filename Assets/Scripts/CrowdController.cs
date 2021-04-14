using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    // Start is called before the first frame update
    public string geneticCode;
    public float[] weights;
    public bool runningUnderGA = true;
    public List<GameObject> Pedestrians;
    public int fitness;
    public GameObject simulation;
    public UnityEngine.UI.Text text;
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
        text.text = $"Genetic Code: {geneticCode.ToString()}\nWeights\nPself: {weights[0]}\nVself: {weights[1]}\nDself: {weights[2]}\nPothers: {weights[3]}\nVothers: {weights[4]}\nDothers: {weights[5]}";
        // InvokeRepeating("UpdateSimulation", 0, repeatRate);
    }

    private void InterpretGeneticCode()
    {
        weights = new float[6];
        long gc = Convert.ToInt64(geneticCode,2);
        int [] intWeights = new int[6];
        long temp = gc;
        const int bitsPerWeight = 64/6;
        for (int i = intWeights.Length - 1; i >= 0 ; i--)
        {
            intWeights[i] = (int) (temp % bitsPerWeight); //floor division
            temp /= bitsPerWeight;
        }
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = ((float)(intWeights[i] - bitsPerWeight/2))/bitsPerWeight; 
        }
        
    }
    public void UpdateFitness(int change)
    {
        fitness += change;
    }
    private bool firstRun = true;
    void FixedUpdate()
    {
        if (firstRun)
        {
            firstRun = false;
        }
        else
        {
            CheckSimulationComplete();
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
}
