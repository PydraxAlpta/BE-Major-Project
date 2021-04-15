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
    public int currentGeneration = 0, maxGenerations = 5;
    public int currentCC = 0, maxCCPerGeneration = 10;
    public int[] fitnessesOfCurrentGeneration;
    public int[] maxFitnessEachGeneration;
    public int maxFitnessObserved = int.MinValue;
    public long maxFitnessGC = -1;
    public const int minFitnessCutoff = -4000;
    void Start()
    {
        children = new long[maxCCPerGeneration];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = (long)UnityEngine.Random.Range(long.MinValue, long.MaxValue);
        }
        fitnessesOfCurrentGeneration = new int[maxCCPerGeneration];
        maxFitnessEachGeneration = new int[maxGenerations];
        crowdController = CCPrefab.GetComponent<CrowdController>();
        CreateNewCCInstance();
    }

    private void CreateNewCCInstance()
    {
        crowdControllerIndividual = Instantiate(crowdController);
        crowdControllerIndividual.runningUnderGA = true;
        crowdControllerIndividual.geneticCode = children[currentCC];
        Debug.Log($"Instance with Genetic Code: {crowdControllerIndividual.geneticCode} started");
        currentCC++;
    }
    private void EndGeneration()
    {
        int fitnessImprovement;
        currentGeneration++;
        for (int i = 0; i < maxCCPerGeneration; ++i)
        {
            if (fitnessesOfCurrentGeneration[i] > maxFitnessObserved)
            {
                maxFitnessObserved = fitnessesOfCurrentGeneration[i];
                maxFitnessGC = children[i];
            }
        }
        maxFitnessEachGeneration[currentGeneration - 1] = maxFitnessObserved;
        if (currentGeneration > 1)
        {
            fitnessImprovement = maxFitnessObserved - maxFitnessEachGeneration[currentGeneration - 2];
            Debug.Log($"Fitness Improvement: {fitnessImprovement} in Generation: {currentGeneration}");
        }
        if (currentGeneration < maxGenerations)
        {
            //do selection crossover mutation stuff
            children = TournamentSelection(children, fitnessesOfCurrentGeneration);
            if (children.Length!=maxCCPerGeneration)
            {
                Debug.LogError($"Tournament Selection returned unequal children than passed {children.Length} != {maxCCPerGeneration}");
            }
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
        // Debug.Log("Update called", this);
        if (crowdControllerIndividual == null)
        {
            // Debug.Log("Crowd controller is null", crowdControllerIndividual);
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
        // Debug.Log("Crowd controller is not null", crowdControllerIndividual);
        if (crowdControllerIndividual.fitness <= minFitnessCutoff)
        {
            crowdControllerIndividual.finishedSimulation = true;
            Debug.Log($"Ending Instance due to low fitness with genetic code: {crowdControllerIndividual.geneticCode}");
        }
        if (crowdControllerIndividual.finishedSimulation)
        {
            fitnessesOfCurrentGeneration[currentCC - 1] = crowdControllerIndividual.fitness;
            // Debug.Log("Reached before logging instance end");
            Debug.Log($"Instance with Genetic Code: {crowdControllerIndividual.geneticCode} finished with fitness: {crowdControllerIndividual.fitness}");
            // Debug.Log("Reached after logging instance end");
            crowdControllerIndividual.EndSimulation();
            crowdControllerIndividual = null;
        }
    }

    public List<long> crossover(string parent1, string parent2)
    {
        //Crossover function takes 2 parents as i/p
        //parents to be selected using some selection method 
        List<long> children_after_crossover = new List<long>();

        parent1 = parent1.PadLeft(8, '0');
        parent2 = parent2.PadLeft(8, '0');

        string crossover_child_1 = parent1.Substring(0, 4) + parent2.Substring(4, 4);
        string crossover_child_2 = parent1.Substring(4, 0) + parent2.Substring(0, 4);
        //python equivalent of parent1[0:4] + parent1[4:8] 
        children_after_crossover.Add(Mutate(crossover_child_1));
        children_after_crossover.Add(Mutate(crossover_child_2));


        return children_after_crossover; //single child string
    }

    public long Mutate(string child) //Mutate to be called after crossover phase
    {
        System.Random r = new System.Random();
        StringBuilder mutate_child;


        mutate_child = new StringBuilder(child);
        int r1 = r.Next(mutate_child.Length);
        int r2 = r.Next(mutate_child.Length);
        if (mutate_child[r1] == '0')
        {
            mutate_child[r1] = '1';
        }
        else
        {
            mutate_child[r1] = '0';
        }
        if (mutate_child[r2] == '0')
        {
            mutate_child[r2] = '1';
        }
        else
        {
            mutate_child[r2] = '0';
        }

        return Convert.ToInt64(mutate_child.ToString(), 2); //return child string after mutate operation
    }
    public long[] TournamentSelection(long[] children, int[] fitnessArray)
    {
        System.Random r = new System.Random();

        long min_child; //child with min fitness value 
        long max_child; //child with max fitness value 


        List<long> selected = new List<long>();

        var children_fitness = new Dictionary<long, int>();

        for (int index = 0; index < children.Length; index++)
        {
            children_fitness.Add(children[index], fitnessArray[index]);

        }

        max_child = children_fitness.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        min_child = children_fitness.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;


        int children_length;
        int count = 0;

        if (children.Length % 2 != 0) //if len(children) == odd; remove child with min fitness value to make len even
        {

            children_fitness.Remove(min_child);
            children = children.Where(val => val != min_child).ToArray();
        }
        children_length = children.Length; //length after removal of child with min fitness
        while (count != children_length)
        {
            count += 2;

            long[] random_child_values;
            random_child_values = children.OrderBy(x => r.Next()).Take(2).ToArray();

            if (children_fitness[random_child_values[0]] >= children_fitness[random_child_values[1]])
            {
                selected.Add(random_child_values[0]);
            }
            else
            {
                selected.Add(random_child_values[1]);
            }

            foreach (var value in random_child_values)
            {
                children = children.Where(val => val != value).ToArray();

            }

        }

        selected.Add(max_child); //add child with max fitness to make array even

        List<long> temp = new List<long>();

        for (int index = 0; index < selected.Count - 1; index++)
        {
            foreach (var crossed_child in crossover(Convert.ToString(selected[index], 2), Convert.ToString(selected[index + 1], 2)))
            {
                temp.Add(crossed_child);
            }

        }

        return temp.ToArray();
    }


}
