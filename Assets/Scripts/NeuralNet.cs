﻿

using System.Collections.Generic;

public class NeuralNet {

    /*
     * A neural net must have
     * 1. Inputs (8 inputs the joints)
     * 2. hidden layers
     * 3. Outputs (8 outputs the joints)
     */

    //list of float[] represent the layers and neurons
    private int[] nnStructure;
    private float[][] neurons;
    private float[][][] weights;
    private float fitnessValue;

public NeuralNet(int[] nnStructure)
    {
        this.nnStructure = new int[nnStructure.Length];
        for (int i = 0; i < nnStructure.Length; i++)
        {
            this.nnStructure[i] = nnStructure[i];
        }

        initaliseNeurons();
        initaliseWeights();
    }

    void initaliseNeurons()
    {
        List<float[]> tempNeurons = new List<float[]>();

        for(int i = 0; i < nnStructure.Length; i++)
        {
            tempNeurons.Add(new float[nnStructure[i]]);
        }

        neurons = tempNeurons.ToArray();
    }

    void initaliseWeights()
    {
        //layer level
        List<float[][]> tempWeights = new List<float[][]>();

        //We start at 1 because the output layer do not need weights
        for (int i = 1; i < nnStructure.Length; i++)
        {
            //Node level
            List<float[]> layerWeights = new List<float[]>();

            //We will give random weights to every neuron in all layers
            //Size of last layer
            int previousNeurons = nnStructure[i -1];

            //Weights layer
            for (int j = 0; j < nnStructure[i]; j++)
            {
                float[] weightsConnections = new float[previousNeurons];

                for(int k = 0; k < previousNeurons; k++)
                {
                    weightsConnections[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeights.Add(weightsConnections);
            }
            tempWeights.Add(layerWeights.ToArray());
        }

        weights = tempWeights.ToArray();
    }
    // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^ structure of a basic NN
    
    
    //Computation vvvvvvvvvvvvvvvvvvvvvv
    //Forward Feed takes in a set of inputs and returns a set of outputs
    public float[] forwardFeed(float[] inputs)
    {

        //we will translate the inputs into the neural net
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //Interate through the rest of the neurons 
        //For the number of layers in the NN loop through
        for(int i = 1; i < nnStructure.Length; i++)
        {
            //For every neuron in the layer, we start at the second layer
            for(int neuronIndex = 0; neuronIndex < nnStructure[i]; neuronIndex++)
            {
                float totalWeight = 0f;
                //for every previous neuron that's connected to the current neuron
                for (int prevNeuronIndex = 0; prevNeuronIndex < nnStructure[i-1]; prevNeuronIndex++)
                {
                    totalWeight += weights[i-1][neuronIndex][prevNeuronIndex];
                }

                neurons[i][neuronIndex] = (float)System.Math.Tanh(totalWeight);
            }
        }

        return null;
    }

    //Should look into ReLU as an activation function

    public void mutate()
    {
        for(int layer = 0; layer < nnStructure.Length-1; layer++)
        {
            for(int neuron = 0; neuron < nnStructure[layer]; neuron++)
            {
                for(int weight = 0; weight< weights[layer][neuron].Length; weight++)
                {
                    float mutatedWeight = weights[layer][neuron][weight];

                    int randomValue = UnityEngine.Random.Range(0, 1000);

                    if(randomValue <= 2)
                    {
                        mutatedWeight = mutatedWeight*-1;
                    }
                    else if(randomValue <= 4)
                    {
                        mutatedWeight = UnityEngine.Random.Range(-1, 1);
                    }
                    else if (randomValue <= 6)
                    {
                        mutatedWeight += mutatedWeight * UnityEngine.Random.Range(0, 1);
                    }
                    else if (randomValue <= 8)
                    {
                        mutatedWeight -= mutatedWeight * UnityEngine.Random.Range(0, 1);
                    }

                    weights[layer][neuron][weight] = mutatedWeight;
                }
            }
        }
    }

    public NeuralNet crossOver(NeuralNet partnerNeuralNet)
    {
        //Might implement this in the future
        return null;
    }

    public float getFitness()
    {
        return fitnessValue;
    }

    public void setFitness(float fitnessValue)
    {
        this.fitnessValue = fitnessValue;
    }

    public void resetFitness()
    {
        fitnessValue = 0;
    }
}