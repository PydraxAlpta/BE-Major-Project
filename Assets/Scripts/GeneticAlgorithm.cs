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
    public const int minFitnessCutoff = -10000;
    public float mutateOdds = 0.1f;
    void Start()
    {
        children = new long[maxCCPerGeneration];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = (long)UnityEngine.Random.Range(long.MinValue, long.MaxValue);
        }
        children[1] = 564048928702976; //:tomsneaky:
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
            long [] tempChildren = new long[maxCCPerGeneration];
            Array.Copy(children,tempChildren,maxCCPerGeneration);
            tempChildren = TournamentSelection(tempChildren, fitnessesOfCurrentGeneration);
            Array.Copy(tempChildren,children,maxCCPerGeneration);
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
            fitnessesOfCurrentGeneration[currentCC - 1] = (crowdControllerIndividual.fitness + crowdControllerIndividual.maxFitness) /2;
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
        int lengthString = 64;
        parent1 = parent1.PadLeft(lengthString, '0');
        parent2 = parent2.PadLeft(lengthString, '0');

        string crossover_child_1 = parent1.Substring(0, lengthString/2) + parent2.Substring(lengthString/2, lengthString/2);
        string crossover_child_2 = parent2.Substring(0, lengthString/2) + parent1.Substring(lengthString/2, lengthString/2);
        //python equivalent of parent1[0:4] + parent1[4:8] 
        children_after_crossover.Add(Mutate(crossover_child_1));
        children_after_crossover.Add(Mutate(crossover_child_2));


        return children_after_crossover; //list of 2 children
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

        // long min_child; //child with min fitness value 
        // long max_child; //child with max fitness value 

        List<long> selected = new List<long>();

        var children_fitness = new Dictionary<long, int>();

        for (int index = 0; index < children.Length; index++)
        {
            if(!children_fitness.ContainsKey(children[index]))
            {
                children_fitness.Add(children[index], fitnessArray[index]);
            }
        }
        long min=0, minValue = long.MaxValue, min2=0, max=0, maxValue = long.MinValue, max2=0;
        foreach (var key in children_fitness.Keys)
        {
            if (children_fitness[key] < minValue)
            {
                min2 = min;
                min = key;
                minValue = children_fitness[key];
            }
            if (children_fitness[key] > minValue)
            {
                max2 = max;
                max = key;
                maxValue = children_fitness[key];
            }
        }
        // max_child = children_fitness.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        // min_child = children_fitness.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;


        // int children_length;
        // // int count = 0;

        // if (children_fitness.Count % 2 != 0) //if len(children) == odd; remove child with min fitness value to make len even
        // {
        //     children_fitness.Remove(min_child);
        //     // children = children.Where(val => val != min_child).ToArray();
        // }
        // children_length = children_fitness.Count; //length after removal of child with min fitness
        // while (count < children_length)
        // {
        //     count += 2;

        //     long[] random_child_keys;
        //     random_child_keys = children_fitness.Keys.OrderBy(x => r.Next()).Take(2).ToArray();

        //     if (children_fitness[random_child_keys[0]] >= children_fitness[random_child_keys[1]])
        //     {
        //         selected.Add(random_child_keys[0]);
        //     }
        //     else
        //     {
        //         selected.Add(random_child_keys[1]);
        //     }
        //     foreach (var key in random_child_keys)
        //     {
        //         children_fitness.Remove(key);
        //     }
        // }

        // selected.Add(max_child); //add child with max fitness to make array even

        //List<long> temp = new List<long>();
        /*
         * Old Logic
        for (int index = 0; index < selected.Count - 1; index++)
        {
            foreach (var crossed_child in crossover(Convert.ToString(selected[index], 2), Convert.ToString(selected[index + 1], 2)))
            {
                temp.Add(crossed_child);
            }
        }
        while(temp.Count<maxCCPerGeneration)
        {
            temp.Add((long)UnityEngine.Random.Range(long.MinValue,long.MaxValue));
        }
        for (int i = 0; i < temp.Count; i++)
        {
            if (r.Next(100) < (int)(mutateOdds*100))
            {
                temp[i] = Mutate(Convert.ToString(temp[i],2));
            }
        }
        */
        //Better Approach: Selecting all the winner parents from Selection and the children of 2 best parents

        // long best_parent_1 = children_fitness.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;

        // children_fitness.Remove(best_parent_1);              //remove the already selected parent with max fitness to avoid repetition

        // long best_parent_2 = children_fitness.Aggregate((a, b) => a.Value > b.Value ? a : b).Key; //parent with 2nd highest fitness

        foreach (var child in crossover(Convert.ToString(max, 2), Convert.ToString(max2, 2)))
        {
            selected.Add(child); //add children of 2 best parents in the selected array. 
        }
        selected.Remove(min);
        selected.Remove(min2);
        while(selected.Count<maxCCPerGeneration)
        {
            selected.Add((long)UnityEngine.Random.Range(long.MinValue,long.MaxValue));
        }
        return selected.ToArray();
    }
}
