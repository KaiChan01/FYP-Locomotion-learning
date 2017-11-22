/* Implementation of the Genetic Algorithm
 * Author: Ka Yu Chan
 * Date: 12/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using UnityEngine;
using System;
using System.Collections.Generic;

public class GeneticAlgorithm {

    public float fitnessSum { get; private set; }
    public int populationSize { get; private set; }
    public int generatation { get; set; }
    public int numOfParentsWanted { get; set; }
    public Individual[] population { get; private set; }
    public Individual fittestSoFar { get; private set; }
    public System.Random rand { get; private set; }
    public float currentHighestFittness { get; private set; }
    public float startingDist { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public int geneSize { get; private set; }

    public int[] bestGene { get; private set; }
    public float bestFitness { get; private set; }

    //Create population and size
    public GeneticAlgorithm(int populationSize, System.Random random, int geneSize, float startingDist, Vector3 targetPosition)
    {
        this.generatation = 1;
        this.populationSize = populationSize;
        this.startingDist = startingDist;
        this.targetPosition = targetPosition;
        this.geneSize = geneSize;
        this.currentHighestFittness = 100;
        this.rand = random;
        this.bestGene = new int[geneSize];
        this.bestFitness = 0;
        this.numOfParentsWanted = 5;

        population = new Individual[populationSize];
    }

    public void populate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new Individual(geneSize, rand, startingDist, targetPosition);
        }
    }

    public void breedNewGeneration(float mutationRate)
    {
        Individual[] newGeneration = new Individual[populationSize];
        List<int> parentsList = chooseFittestParents(numOfParentsWanted);

        for (int i = 0; i < populationSize / 2; i++)
        {
            int chooseParent = UnityEngine.Random.Range(0, numOfParentsWanted - 1);
            int chooseParent2 = UnityEngine.Random.Range(0, numOfParentsWanted - 1);
            Individual parent1 = population[parentsList[chooseParent]];
            Individual parent2 = population[parentsList[chooseParent2]];
            Individual child1 = parent1.crossover(parent2);
            Individual child2 = parent2.crossover(parent1);

            child1.mutate(mutationRate);
            child2.mutate(mutationRate);

            newGeneration[i * 2] = child1;
            newGeneration[(i * 2) + 1] = child2;
        }
        population = newGeneration;
        generatation++;
    }

    //Need to choose the best parents
    public void calculateTotalFitness()
    {
        fitnessSum = 0;
        for(int i = 0; i < populationSize; i++)
        {
            fitnessSum += population[i].fitnessValue;
            if(population[i].fitnessValue > bestFitness)
            {
                bestFitness = population[i].fitnessValue;
                bestGene = (int[]) population[i].genes.Clone();
                Debug.Log(bestFitness);
            }
        }
    }

    public List<int> chooseFittestParents(int numOfParentsWanted)
    {
        List<int> parentIndex = new List<int>();
        float highestFitness = 0;

        for (int j = 0; j < numOfParentsWanted; j++)
        {
            for (int i = 0; i < populationSize; i++)
            {
                if (highestFitness < population[i].fitnessValue && parentIndex.IndexOf(i) == -1)
                {
                    parentIndex.Add(i);
                }
                //This way the better ones will more likely be choosen first
            }
        }
        return parentIndex;
    }

    /*
    public void mutatePopulation(System.Random rand)
    {
        for (int i = 0; i < populationSize-1; i++)
        {
            //We keep one of the fittest one
            population[i].mutate(rand.Next(0, 2));
        }
    }
    
    public void chooseBaseForNextGeneration()
    {
        //Set a random varible to find the the best parents first
        int indexOfFittest = 0;
        //The fitness is the closest to the target
        float highestFitness = 100;
        for (int i = 0; i < populationSize; i++)
        {
            if (highestFitness >= population[i].fitnessValue)
            {
                highestFitness = population[i].fitnessValue;
                indexOfFittest = i;
            }
            //This way the better ones will more likely be choosen first
        }

        Individual tempIndividual = population[indexOfFittest];

        if(tempIndividual.fitnessValue < currentHighestFittness)
        {
            currentHighestFittness = tempIndividual.fitnessValue;
            Debug.Log("new fittest");
            fittestSoFar = new Individual(tempIndividual);
            for (int i = 0; i < populationSize; i++)
            {
                population[i] = new Individual(fittestSoFar);
            }
            Debug.Log("Mutating new fittest canidate population with fitness: " + fittestSoFar.fitnessValue + " Generation: " + generatation);
        }
        else
        {
            for (int i = 0; i < populationSize; i++)
            {
                population[i] = new Individual(fittestSoFar);
            }
            Debug.Log("Mutating same population" + " Generation: " + generatation);
            Debug.Log(fittestSoFar.fitnessValue);
        }
        generatation++;
    }
    */
}
