/* Implementation of the Genetic Algorithm
 * Author: Ka Yu Chan
 * Date: 12/10/2017
 * 
 * Framework referenced from https://www.youtube.com/watch?v=G8KJWONEeGo&t=450s (25/10/17)
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class GeneticAlgorithm
{

    public int populationSize { get; private set; }
    public int generatation { get; private set; }
    public NeuralNet[] population { get; private set; }
    public List<NeuralNet> bestParents = new List<NeuralNet>();
    public int mutationRate;

    public int runLimit;
    public float highestFromGeneration;

    private int[] neuralStructure;
    private string trainingName;

    private string logsFilePath = "/Logs/";
    private string logfileName = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
    private string trainedFilePath = "/TrainedNetworks/";

    //Create population and size
    public GeneticAlgorithm(int populationSize, int[] neuralStructure, int runLimit, int mutationRate, string trainingName)
    {
        this.generatation = 0;
        this.populationSize = populationSize;
        this.neuralStructure = neuralStructure;
        this.runLimit = runLimit;
        population = new NeuralNet[populationSize];
        createNewGeneration();
        this.mutationRate = mutationRate;
        this.trainingName = trainingName;
    }


    public void createNewGeneration()
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


    public float calculateTotalFitness()
    {
        float fitnessSum = 0;
        for (int i = 0; i < populationSize; i++)
        {
            fitnessSum += population[i].getFitness();
        }

        return fitnessSum;
    }

    //This needs to be changed
    public int[] chooseParents(int numOfParentsWanted)
    {
        //List to store the index of the fittest parents
        List<int> parentIndex = new List<int>();

        for (int j = 0; j < numOfParentsWanted; j++)
        {
            //Temparary highest fitness 
            float highestFitness = population[0].getFitness();

            int highestIndex = 0;

            //We can start the comparision at index 1 
            for (int i = 1; i < populationSize; i++)
            {
                //If the creature's fitness is new highest and isn't already in array then update the current highestIndex
                if (highestFitness <= population[i].getFitness() && parentIndex.IndexOf(i) == -1)
                {
                    highestFitness = population[i].getFitness();
                    highestIndex = i;
                }
                //This way the better ones will more likely be choosen first
            }

            //Add the new found highest Index to the parentIndex Array
            parentIndex.Add(highestIndex);

            //The first parent found should be the fittest parent in this generation
            if (j == 0)
            {
                highestFromGeneration = population[parentIndex[0]].getFitness();
            }
        }

        return parentIndex.ToArray();
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
        
        //Clone the best parents from the remainder of the population
        for (int i = parentIndexArray.Length; i < populationSize; i++)
        {
            //Copy best parents
            int randParentIndex = UnityEngine.Random.Range(0, numOfParentsWanted);
            newPopulation[i] = new NeuralNet(newPopulation[randParentIndex]);
        }
        population = newPopulation;
    }

    //Used for the random testing phase, adds best parent of the generation during the phase
    public void addBestParent()
    {
        bestParents.Add(selectBestNeuralNetFromCurrentGen());
    }

    //Re-plays and save the best parent into JSON
    public void showcaseAndSaveBestParent()
    {
        NeuralNet[] newPopulation = new NeuralNet[populationSize];
        float highestFitness = population[0].getFitness() ;
        int fittestIndex = 0;

        for (int i = 1; i < populationSize; i++)
        {
            if (highestFitness < population[i].getFitness())
            {
                highestFitness = population[i].getFitness();
                fittestIndex = i;
            }
        }

        //Every creature copies the best individual from the run
        for (int i = 0; i < populationSize; i++)
        {
            newPopulation[i] = new NeuralNet(population[fittestIndex]);
        }

        //Persis the network into JSON formate
        saveNetwork(population[fittestIndex]);

        population = newPopulation;
    }


    //Testing all the best parents from the random phase
    public void testAllInitialBest()
    {
        NeuralNet[] bestArray = bestParents.ToArray();
        for(int i = 0; i < bestArray.Length; i++)
        {
            population[i] = bestArray[i];
        }

        //Might remove this
        if(bestArray.Length < populationSize)
        {
            for(int i = bestArray.Length; i < populationSize; i++)
            {
                population[i] = new NeuralNet(neuralStructure);
            }
        }
    }


    //We mutate all the cloned individuals, we leave the original alone to prevent regression
    public void mutatePopulation(int numParentsChoosen)
    {
        for (int i = numParentsChoosen; i < populationSize; i++)
        {
                population[i].mutate(mutationRate);
        }
    }



    public NeuralNet selectBestNeuralNetFromCurrentGen()
    {
        float tempHighestFitness = population[0].getFitness();
        int bestIndex = 0;

        for (int i = 1; i < populationSize; i++)
        {
            float currentFitness = population[i].getFitness();
            if (currentFitness > tempHighestFitness)
            {
                tempHighestFitness = currentFitness;
                bestIndex = i;
            }
        }
        
        NeuralNet bestIndividual = population[bestIndex];
        highestFromGeneration = bestIndividual.getFitness();
        return bestIndividual;
    }


    //Saves the given network
    public void saveNetwork(NeuralNet network)
    {
        TrainedNetwork netToSave = new TrainedNetwork();
        netToSave.nnStructure = network.getStructure();
        netToSave.weights = network.flattenWeightsToArray();
        netToSave.generation = generatation;

        string networkAsJSON = JsonUtility.ToJson(netToSave);
        string filePath = Application.dataPath + trainedFilePath + trainingName + "_" + generatation + ".json";
        File.WriteAllText(filePath, networkAsJSON);
    }


    
    public float calculateAverageFitness()
    {
        float average = (calculateTotalFitness() / populationSize);
        return average;
    }



    public float calculateHighestFitness()
    {
        float highest = population[0].getFitness();
        for (int i = 1; i < populationSize; i++)
        {
            float currentFitness = population[i].getFitness();
            if (highest < currentFitness)
            {
                highest = currentFitness;
            }
        }
        return highest;
    }



    public void saveLog()
    {
        string newFileName = Application.dataPath + logsFilePath + logfileName + ".csv";

        string genertationDetails = generatation + "," + calculateAverageFitness() + "," + calculateHighestFitness() + "\n";


        if (!File.Exists(newFileName))
        {
            string logHeaders = "GenerationNum" + "," + "AverageFitness" + "," + "HighestFitness" + "\n";

            File.WriteAllText(newFileName, logHeaders);
        }

        File.AppendAllText(newFileName, genertationDetails);
    }



    public void incrementGeneration()
    {
        generatation++;
    }
}