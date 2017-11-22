/* A class that represents an indiviual that makes up the population in the genetic algorithm
 * Author: Ka Yu Chan
 * Date: 11/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using System;
using UnityEngine;

public class Individual {

    //public GeneType[] genes { get; private set; }
    public int[] genes { get; private set; }
    public float fitnessValue { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public float startingDist { get; private set; }
    public int geneSize { get; private set; }
    public System.Random rand { get; private set; }

    public Individual(int geneSize, System.Random random, float startingDist, Vector3 targetPosition)
    {
        this.geneSize = geneSize;
        //this.genes = new GeneType[geneSize];
        this.genes = new int[geneSize];
        this.startingDist = startingDist;
        this.targetPosition = targetPosition;
        this.rand = random;

        //Create array of Gene
        for (int i = 0; i < geneSize; i++)
        {
            genes[i] = createRandomGene();
        }
    }

    public Individual(Individual oneForCopy)
    {
        this.geneSize = oneForCopy.geneSize;
        this.genes = new int[geneSize];
        this.startingDist = oneForCopy.startingDist;
        this.targetPosition = oneForCopy.targetPosition;
        this.fitnessValue = oneForCopy.fitnessValue;
        this.rand = oneForCopy.rand;
        this.genes = oneForCopy.genes;
    }

    public void mutate(float mutationRate)
    {
        //Might change the way this mutates 
        for (int i = 0; i < genes.Length; i++)
        {
            if (rand.NextDouble() < mutationRate)
            {
                genes[i] = createRandomGene();
            }
        }
    }

    public int createRandomGene()
    {
        return UnityEngine.Random.Range(0, 3);
    }

    public float calculateFitness(Vector3 currentPosition, Vector3 targetPosition)
    {
        fitnessValue = startingDist -  Vector3.Distance(currentPosition, targetPosition);
        if(fitnessValue < 0)
        {
            fitnessValue = 0;
        }
        return fitnessValue;
    }

    public Individual crossover(Individual partner)
    {
        //Child should have the same gene size as parent
        Individual child = new Individual(genes.Length, rand, startingDist, targetPosition);

        // 50 % chance of getting genes between parents
        for (int i = 0; i < genes.Length; i++)
        {
            child.genes[i] = rand.NextDouble() < 0.5f ? genes[i] : partner.genes[i];
        }

        return child;
    }

    /*
    private void copyGenes(GeneType[] genesToCopy)
    {
        for(int i = 0; i < geneSize; i++)
        {
            this.genes[i] = new GeneType(genesToCopy[i]);
        }
    }
    

    public GeneType createRandomGene()
    {
        //Hard coding these values for now
        GeneType gene = new GeneType(150, 200, 0.02f);
        return gene;
    }

    //This approach is to try and mutate the creature and learn that way rather than crossbreeding
    public void mutate(int geneIndex)
    {
        genes[geneIndex] = createRandomGene();
    }

    */

    //What determines fitness?
}
