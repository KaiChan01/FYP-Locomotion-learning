/* Implementation of the Genetic Algorithm
 * Author: Ka Yu Chan
 * Date: 12/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using UnityEngine;
using System;

public class GeneticAlgorithm {

    public float fitness { get; private set; }
    public int populationSize;
    public int generatation { get; private set; }
    public Individual[] population { get; private set; }
    private System.Random random;

    private float fitnessSum;
    private float mutationRate;
    private float startingDist;
    private Vector3 targetPosition;
    private int geneSize;

    //Create population and size
    public GeneticAlgorithm(int populationSize, int geneSize, System.Random rand, float mutationRate, float startingDist, Vector3 targetPosition)
    {
        this.generatation = 1;
        this.random = rand;
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.startingDist = startingDist;
        this.targetPosition = targetPosition;
        this.geneSize = geneSize;

        population = new Individual[populationSize];
    }

    public void populate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new Individual(geneSize, random, startingDist, targetPosition);
        }
    }
    
    public void breedNewGeneration()
    {
        Individual[] newGeneration = new Individual[populationSize];
        for (int i = 0; i < populationSize / 2; i++)
        {
            Individual parent1 = chooseParent();
            Individual parent2 = chooseParent();
            Individual child1 = parent1.crossover(parent2);
            Individual child2 = parent2.crossover(parent1);

            child1.mutate(mutationRate);
            child2.mutate(mutationRate);

            newGeneration[i * 2] = child1;
            newGeneration[(i * 2) +1] = child2;
        }
        population = newGeneration;
        generatation++;
    }

    //Need to choose the best parents

    public void calculateTotalFitness(float startingDist)
    {
        fitnessSum = 0;
        for(int i = 0; i < populationSize; i++)
        {
            fitnessSum += startingDist - population[i].fitnessValue;
        }
    }


    //This needs to be changed
    public Individual chooseParent()
    {
        //Set a random varible to find the the best parents first
        double fitnessLevel = random.NextDouble() * fitnessSum;
        for (int i = 0; i < populationSize; i++)
        {
            if (fitnessLevel >= population[i].fitnessValue)
            {
                return population[i];
            }
            //This way the better ones will more likely be choosen first
            fitnessLevel += population[i].fitnessValue;
        }
        return null;
    }
}
