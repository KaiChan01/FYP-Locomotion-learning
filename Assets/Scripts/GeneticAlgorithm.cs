/* Implementation of the Genetic Algorithm
 * Author: Ka Yu Chan
 * Date: 12/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using UnityEngine;
using System;
using System.Collections.Generic;

public class GeneticAlgorithm
{

    public float fitness { get; private set; }
    public int populationSize { get; private set; }
    public int generatation { get; private set; }
    public NeuralNet[] population { get; private set; }
    public int mutationRate;

    private float fitnessSum;
    private int[] neuralStructure;

    //Create population and size
    public GeneticAlgorithm(int populationSize, int[] neuralStructure)
    {
        this.generatation = 0;
        this.populationSize = populationSize;
        this.neuralStructure = neuralStructure;
        population = new NeuralNet[populationSize];

        mutationRate = 100;
    }

    public void populate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new NeuralNet(neuralStructure);
        }
    }

    public void breedNewGeneration()
    {
        NeuralNet[] newGeneration = new NeuralNet[populationSize];
        for (int i = 0; i < populationSize / 2; i++)
        {
            NeuralNet parent1 = chooseParent();
            NeuralNet parent2 = chooseParent();
            NeuralNet child1 = parent1.crossOver(parent2);
            NeuralNet child2 = parent2.crossOver(parent1);

            child1.mutate(mutationRate);
            child2.mutate(mutationRate);

            newGeneration[i * 2] = child1;
            newGeneration[(i * 2) + 1] = child2;
        }
        population = newGeneration;
        generatation++;
    }

    //Need to choose the best parents

    public void calculateTotalFitness(float startingDist)
    {
        fitnessSum = 0;
        for (int i = 0; i < populationSize; i++)
        {
            fitnessSum += startingDist - population[i].getFitness();
        }
    }

    //This needs to be changed
    public NeuralNet chooseParent()
    {
        //Set a random varible to find the the best parents first
        double fitnessLevel = UnityEngine.Random.Range(0,1) * fitnessSum;
        for (int i = 0; i < populationSize; i++)
        {
            if (fitnessLevel >= population[i].getFitness())
            {
                return population[i];
            }
            //This way the better ones will more likely be choosen first
            fitnessLevel += population[i].getFitness();
        }
        return null;
    }

    public void replaceOldParentWithNew(int numOfParentsWanted)
    {
        List<int> parentIndex = new List<int>();

        for (int j = 0; j < numOfParentsWanted; j++)
        {
            float highestFitness = 0;
            int highestIndex = -1;
            for (int i = 0; i < populationSize; i++)
            {
                if (highestFitness <= population[i].getFitness() && parentIndex.IndexOf(i) == -1)
                {
                    highestFitness = population[i].getFitness();
                    highestIndex = i;
                }
                //This way the better ones will more likely be choosen first
            }
            parentIndex.Add(highestIndex);
        }

        int[] parentIndexArray = parentIndex.ToArray();

        NeuralNet[] newPopulation = new NeuralNet[populationSize];

        //Top parents are sure to be in the new population
        for (int i = 0; i < parentIndexArray.Length; i++)
        {
            newPopulation[i] = new NeuralNet(population[parentIndexArray[i]]);
            Debug.Log(i);
        }
        
        for (int i = parentIndexArray.Length; i < populationSize; i++)
        {
            //alternate between the best parents
            newPopulation[i] = new NeuralNet(newPopulation[(i - parentIndexArray.Length) % parentIndexArray.Length]);
        }
        

        population = newPopulation;
    }

    public void mutatePopulation()
    {
        if(generatation == 10)
        {
            mutationRate = 1000;
        }
        else if(mutationRate >= 1000 && (generatation % 10) == 0)
        {
            mutationRate += 100;
        }

        //We do not mutate the top parents, this way they're consistant
        for (int i = 2; i < populationSize; i++)
        {
            population[i].mutate(mutationRate);
        }
    }

    public void incrementGeneration()
    {
        generatation++;
    }
}