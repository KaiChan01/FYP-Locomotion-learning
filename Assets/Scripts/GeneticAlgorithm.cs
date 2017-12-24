/* Implementation of the Genetic Algorithm
 * Author: Ka Yu Chan
 * Date: 12/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using UnityEngine;
using System;

public class GeneticAlgorithm
{

    public float fitness { get; private set; }
    public int populationSize { get; private set; }
    public int generatation { get; private set; }
    public NeuralNet[] population { get; private set; }

    private float fitnessSum;
    private int[] neuralStructure;

    //Create population and size
    public GeneticAlgorithm(int populationSize, int[] neuralStructure)
    {
        this.generatation = 1;
        this.populationSize = populationSize;
        this.neuralStructure = neuralStructure;
        population = new NeuralNet[populationSize];
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

            child1.mutate();
            child2.mutate();

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

    public void mutatePopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            population[i].mutate();
        }
    }
}