using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
public class GeneticAlgorithm : MonoBehaviour
{
    int[] children = { 50, 35, 100, 200, 10 };
    void Start()
    {
        
    }
    public string crossover(string parent1, string parent2)
    {
        //Crossover function takes 2 parents as i/p
        //parents to be selected using some selection method 

        parent1 = parent1.PadLeft(8, '0');
        parent2 = parent2.PadLeft(8, '0');

        string crossoverChild = parent1.Substring(0, 4) + parent2.Substring(4, 4);
        //python equivalent of parent1[0:4] + parent1[4:8] 

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
