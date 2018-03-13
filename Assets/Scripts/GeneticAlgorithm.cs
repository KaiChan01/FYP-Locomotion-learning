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
    public List<NeuralNet> bestParents = new List<NeuralNet>();
    public int mutationRate;

    public int runLimit;
    public int randomPhase;
    public float highestFromGeneration;

    private float fitnessSum;
    private int[] neuralStructure;

    //Create population and size
    public GeneticAlgorithm(int populationSize, int[] neuralStructure, int runLimit, int randomPhase, int mutationRate)
    {
        this.generatation = 0;
        this.populationSize = populationSize;
        this.neuralStructure = neuralStructure;
        this.runLimit = runLimit;
        population = new NeuralNet[populationSize];
        createRandomGeneration();
        this.mutationRate = mutationRate;
        this.randomPhase = randomPhase;
    }

    public void createRandomGeneration()
    {
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new NeuralNet(neuralStructure);
        }
    }

    public void breedNewGeneration(int numOfParentsWanted)
    {
        NeuralNet[] newGeneration = new NeuralNet[populationSize];

        int[] parentIndex = chooseParents(numOfParentsWanted);

        for (int i = 0; i < populationSize / 2; i++)
        {
            int parent1Index = UnityEngine.Random.Range(0, parentIndex.Length);
            int parent2Index = UnityEngine.Random.Range(0, parentIndex.Length);

            NeuralNet parent1 = population[parent1Index];
            NeuralNet parent2 = population[parent2Index];
            NeuralNet child1 = parent1.crossOver(parent2);
            NeuralNet child2 = parent2.crossOver(parent1);

            child1.mutate(mutationRate);
            child2.mutate(mutationRate);

            newGeneration[i * 2] = child1;
            newGeneration[(i * 2) + 1] = child2;
        }
        population = newGeneration;
    }

    //Need to choose the best parents

    public void calculateTotalFitness()
    {
        fitnessSum = 0;
        for (int i = 0; i < populationSize; i++)
        {
            fitnessSum += population[i].getFitness();
        }
    }

    //This needs to be changed
    public int[] chooseParents(int numOfParentsWanted)
    {
        //List to store the index of the fittest parents
        List<int> parentIndex = new List<int>();

        for (int j = 0; j < numOfParentsWanted; j++)
        {
            float highestFitness = -100000;

            //TODO
            //The BUG happens here, when the fit creature's performance isn't consistant
            int highestIndex = -1;

            for (int i = 0; i < populationSize; i++)
            {
                //If the creature's fitness is new highest and isn't already in array
                if (highestFitness <= population[i].getFitness() && parentIndex.IndexOf(i) == -1)
                {
                    highestFitness = population[i].getFitness();
                    highestIndex = i;
                }
                //This way the better ones will more likely be choosen first

                //We will save the highest from this generation

            }
            parentIndex.Add(highestIndex);
            if (j == 0)
            {
                highestFromGeneration = population[parentIndex[0]].getFitness();
            }
        }

        return parentIndex.ToArray();
    }

    public int chooseBestIndividual()
    {
        float highestFitness = -100000;

        int highestIndex = -1;

        for (int i = 0; i < populationSize; i++)
        {
            //If the creature's fitness is new highest and isn't already in array
            if (highestFitness <= population[i].getFitness())
            {
                highestFitness = population[i].getFitness();
                highestIndex = i;
            }
        }
        highestFromGeneration = population[highestIndex].getFitness();
        return highestIndex;
    }

    public void replaceOldParentWithNew(int numOfParentsWanted)
    {

        int[] parentIndexArray = chooseParents(numOfParentsWanted);

        NeuralNet[] newPopulation = new NeuralNet[populationSize];

        //Top parents are sure to be in the new population
        for (int i = 0; i < parentIndexArray.Length; i++)
        {
            newPopulation[i] = new NeuralNet(population[parentIndexArray[i]]);
        }
        
        for (int i = parentIndexArray.Length; i < populationSize; i++)
        {
            //Copy best parents
            int ranParent = UnityEngine.Random.Range(0, numOfParentsWanted);
            newPopulation[i] = new NeuralNet(newPopulation[ranParent]);
        }
        population = newPopulation;
    }

    public void addBestParent()
    {
        bestParents.Add(population[chooseBestIndividual()]);
    }

    public void showcaseBestParent()
    {
        NeuralNet[] newPopulation = new NeuralNet[populationSize];
        float highestFitness = 0;
        int fittestIndex = -1;

        for (int i = 0; i < populationSize; i++)
        {
            if (highestFitness <= population[i].getFitness())
            {
                highestFitness = population[i].getFitness();
                 fittestIndex = i;
            }
        }

        for (int i = 0; i < populationSize; i++)
        {
            newPopulation[i] = new NeuralNet(population[fittestIndex]);
        }

        population = newPopulation;
    }

    public void testAllInitialBest()
    {
        NeuralNet[] bestArray = bestParents.ToArray();
        for(int i = 0; i < bestArray.Length; i++)
        {
            population[i] = bestArray[i];
        }

        if(bestArray.Length < populationSize)
        {
            for(int i = bestArray.Length; i < populationSize; i++)
            {
                population[i] = new NeuralNet(neuralStructure);
            }
        }
    }

    public void mutatePopulation(int numParentsChoosen)
    {
        for (int i = numParentsChoosen; i < populationSize; i++)
        {
                population[i].mutate(mutationRate);
        }
    }

    public void incrementGeneration()
    {
        generatation++;
    }
}