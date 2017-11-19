/* A class that represents an indiviual that makes up the population in the genetic algorithm
 * Author: Ka Yu Chan
 * Date: 11/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using System;
using UnityEngine;

public class Individual {

    public GeneType[] genes { get; private set; }
    public float fitnessValue { get; private set; }
    private Vector3 targetPosition;
    private System.Random random;
    private float startingDist;
    private int geneSize;

    public Individual(int geneSize, System.Random random, float startingDist, Vector3 targetPosition)
    {
        this.geneSize = geneSize;
        this.genes = new GeneType[geneSize];
        this.random = random;
        this.startingDist = startingDist;
        this.targetPosition = targetPosition;

        //Create array of Gene
        for (int i = 0; i < geneSize; i++)
        {
            genes[i] = createRandomGene();
        }
    }

    public GeneType createRandomGene()
    {
        //Hard coding these values for now
        GeneType gene = new GeneType(150, 200, 0.02f);
        return gene;
    }

    /*
    public Individual crossover(Individual partner)
    {
    //Child should have the same gene size as parent
    Individual child = new Individual(genes.Length, random, startingDist, targetPosition);

        // 50 % chance of getting genes between parents
        for(int i = 0; i < genes.Length; i++)
        {
            child.genes[i] = random.NextDouble() * 1 < 0.5f ? genes[i] : partner.genes[i];
        }

        return child;
    }
    

    public void mutate(float mutationRate)
    {
        //Might change the way this mutates 
        for(int i = 0; i < genes.Length; i++)
        {
            if(random.NextDouble() < mutationRate)
            {
                genes[i] = createRandomGene();
            }
        }
    }
    */

    //This approach is to try and mutate the creature and learn that way rather than crossbreeding
    public void mutate(int geneIndex)
    {
        genes[geneIndex] = createRandomGene();
    }


    //What determines fitness?
    public float calculateFitness(Vector3 currentPosition)
    {
        fitnessValue = Vector3.Distance(currentPosition, targetPosition);
        return fitnessValue;
    }
}
