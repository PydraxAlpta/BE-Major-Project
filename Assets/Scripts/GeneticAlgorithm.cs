using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
public class GeneticAlgorithm : MonoBehaviour
{
    public long[] children;
    public GameObject CCPrefab;
    CrowdController crowdController, crowdControllerIndividual;
    public int currentGeneration = 0, maxGenerations = 2;
    public int currentCC = 0, maxCCPerGeneration = 5;
    public int[] fitnessesOfCurrentGeneration;
    public int maxFitnessObserved = int.MinValue;
    public long maxFitnessGC = -1;
    public const int minFitnessCutoff = -4000;
    void Start()
    {
        children = new long[maxCCPerGeneration];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = (long)UnityEngine.Random.Range(0, long.MaxValue);
        }
        fitnessesOfCurrentGeneration = new int[maxCCPerGeneration];
        crowdController = CCPrefab.GetComponent<CrowdController>();
        CreateNewCCInstance();
    }

    private void CreateNewCCInstance()
    {
        crowdControllerIndividual = Instantiate(crowdController);
        crowdControllerIndividual.runningUnderGA = true;
        crowdControllerIndividual.geneticCode = Convert.ToString(children[currentCC], 2);
        Debug.Log($"Instance with Genetic Code: {crowdControllerIndividual.geneticCode} started");
        currentCC++;
    }
    private void EndGeneration()
    {
        currentGeneration++;
        for (int i = 0; i < maxCCPerGeneration; ++i)
        {
            if (fitnessesOfCurrentGeneration[i] > maxFitnessObserved)
            {
                maxFitnessObserved = fitnessesOfCurrentGeneration[i];
                maxFitnessGC = children[i];
            }
        }
        if (currentGeneration < maxGenerations)
        {
            //do selection crossover mutation stuff

            //and start next Generation
            currentCC = 0;
            return;
        }
        else
        {
            Debug.Log($"MAximum fitness observed: {maxFitnessObserved} with Genetic Code {maxFitnessGC}");
            if (Application.isEditor)
            {
                #if (UNITY_EDITOR)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                #endif
            }
            else
            {
                Application.Quit();
            }
        }
    }
    void Update()
    {
        Debug.Log("Update called", this);
        if (crowdControllerIndividual == null)
        {
            Debug.Log("Crowd controller is null", crowdControllerIndividual);
            if (currentCC < maxCCPerGeneration)
            {
                CreateNewCCInstance();
            }
            else
            {
                EndGeneration();
            }
            return;
        }
        Debug.Log("Crowd controller is not null", crowdControllerIndividual);
        if (crowdControllerIndividual.fitness <= minFitnessCutoff)
        {
            crowdControllerIndividual.finishedSimulation = true;
            Debug.Log($"Ending Instance due to low fitness with genetic code: {crowdControllerIndividual.geneticCode}");
        }
        if (crowdControllerIndividual.finishedSimulation)
        {
            fitnessesOfCurrentGeneration[currentCC-1] = crowdControllerIndividual.fitness;
            Debug.Log("Reached before logging instance end");
            Debug.Log($"Instance with Genetic Code: {crowdControllerIndividual.geneticCode} finished with fitness: {crowdControllerIndividual.fitness}");
            Debug.Log("Reached after logging instance end");
            crowdControllerIndividual.EndSimulation();
            crowdControllerIndividual = null;
        }
    }
    public string crossover(string parent1, string parent2)
    {
        //Crossover function takes 2 parents as i/p
        //parents to be selected using some selection method 

        parent1 = parent1.PadLeft(8, '0');
        parent2 = parent2.PadLeft(8, '0');

        string crossoverChild = parent1.Substring(0, 4) + parent2.Substring(4, 4);
        //python equivalent of parent1[0:4] + parent2[4:8] 

        return crossoverChild; //single child string
    }

    public string[] Mutate() //Mutate to be called after crossover phase
    {
        string[] geneticCodes = new string[children.Length];
        System.Random r = new System.Random();
        for (int i = 0; i < children.Length; i++)
        {

            StringBuilder temp = new StringBuilder(Convert.ToString(children[i], 2));
            Debug.Log(i.ToString() + "th " + "Gene Before Mutation :" + temp);
            int r1 = r.Next(temp.Length);
            int r2 = r.Next(temp.Length);

            if (temp[r1] == '0')
            {
                temp[r1] = '1';
            }
            else
            {
                temp[r1] = '0';
            }
            if (temp[r2] == '0')
            {
                temp[r2] = '1';
            }
            else
            {
                temp[r2] = '0';
            }
            geneticCodes[i] = temp.ToString();
            Debug.Log(i.ToString() + "th " + "Gene After Mutation :" + geneticCodes[i]);

        }
        return geneticCodes; //string <arr> of mutated children
    }
}
