/* Implementation of the Neural Network
 * Author: Ka Yu Chan
 * Date: 05/04/2018
 * 
 * Framework referenced from:
 * Title: Tutorial On Programming An Evolving Neural Network In C# w/ Unity3D
 * Author: ChannelName - The One
 * Date: 05/04/18
 * Availability: https://www.youtube.com/watch?v=Yq0SfuiOVYE
 */

using System.Collections.Generic;
using UnityEngine;

public class NeuralNet
{
    /*
     * A neural net must have
     * 1. Inputs (creature Status)
     * 2. hidden layers
     * 3. Outputs (creature actions)
     */

    //list of float[] represent the layers and neurons
    private int[] nnStructure;
    private float[][] neurons;
    private float[][][] weights;
    private float fitnessValue;

    //For un-serializing
    public int generation = 0;

    // Default constructor
    public NeuralNet(int[] nnStructure)
    {
        this.nnStructure = new int[nnStructure.Length];
        this.nnStructure = nnStructure;

        initaliseNeurons();
        initaliseWeights();
    }

    // contructor from flattened serialized weights
    public NeuralNet(float[] flatternedWeights, int[] nnStructure, int generation)
    {
        this.nnStructure = new int[nnStructure.Length];
        this.nnStructure = nnStructure;
        this.generation = generation;

        initaliseNeurons();
        unflatternWeights(flatternedWeights);
    }

    //Copying a NeuralNet
    public NeuralNet(NeuralNet netForCopy)
    {
        this.nnStructure = new int[netForCopy.nnStructure.Length];
        this.nnStructure = netForCopy.nnStructure;

        initaliseNeurons();
        copyNeurons(netForCopy.neurons);
        copyWeights(netForCopy.weights);
    }

    // For Crossover (Not very effective in this approach)
    public NeuralNet(int[] parentStruct, float[][][] parent1Weights, float[][][] parent2Weights)
    {
        this.nnStructure = new int[parentStruct.Length];
        this.nnStructure = parentStruct;

        //Layout of the neurons initialised
        initaliseNeurons();

        //Exchange weights from parents
        List<float[][]> tempWeights = new List<float[][]>();

        for (int i = 1; i < nnStructure.Length; i++)
        {
            //Node level
            List<float[]> layerWeights = new List<float[]>();

            int previousNeurons = nnStructure[i - 1];

            //Weights layer
            for (int j = 0; j < nnStructure[i]; j++)
            {
                float[] weightsConnections = new float[previousNeurons];

                for (int k = 0; k < previousNeurons; k++)
                {
                    //Choose from parent
                    if (Random.Range(0, 2) == 0)
                    {
                        weightsConnections[k] = parent1Weights[i - 1][j][k];
                    }
                    else
                    {
                        weightsConnections[k] = parent2Weights[i - 1][j][k];
                    }

                }

                layerWeights.Add(weightsConnections);
            }
            tempWeights.Add(layerWeights.ToArray());
        }

        weights = tempWeights.ToArray();
    }

    //Create 2D neuron array
    void initaliseNeurons()
    {
        List<float[]> tempNeurons = new List<float[]>();

        for (int i = 0; i < nnStructure.Length; i++)
        {
            tempNeurons.Add(new float[nnStructure[i]]);
        }

        neurons = tempNeurons.ToArray();
    }

    // unflatten serialized weights
    void unflatternWeights(float[] flatternedWeights)
    {
        int indexer = 0;
        List<float[][]> tempWeights = new List<float[][]>();

        for (int i = 1; i < nnStructure.Length; i++)
        {
            List<float[]> layerWeights = new List<float[]>();

            int previousNeurons = nnStructure[i - 1];

            //Weights layer
            for (int j = 0; j < nnStructure[i]; j++)
            {
                float[] weightsConnections = new float[previousNeurons];

                for (int k = 0; k < previousNeurons; k++)
                {
                    weightsConnections[k] = flatternedWeights[indexer];
                    indexer++;
                }

                layerWeights.Add(weightsConnections);
            }
            tempWeights.Add(layerWeights.ToArray());
        }

        weights = tempWeights.ToArray();
    }

    //Create 3D weights array
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
            int previousNeurons = nnStructure[i - 1];

            //Weights layer
            for (int j = 0; j < nnStructure[i]; j++)
            {
                float[] weightsConnections = new float[previousNeurons];

                for (int k = 0; k < previousNeurons; k++)
                {
                    weightsConnections[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeights.Add(weightsConnections);
            }
            tempWeights.Add(layerWeights.ToArray());
        }

        weights = tempWeights.ToArray();
    }

    // Deep copy neurons
    void copyNeurons(float[][] neurons)
    {
        for (int layer = 0; layer < neurons.Length; layer++)
        {
            for (int neuronIndex = 0; neuronIndex < neurons[layer].Length; neuronIndex++)
            {
                this.neurons[layer][neuronIndex] = neurons[layer][neuronIndex];
            }
        }
    }

    // Deep copy weights
    void copyWeights(float[][][] weightsCopy)
    {
        List<float[][]> tempWeights = new List<float[][]>();

        for (int i = 1; i < nnStructure.Length; i++)
        {
            List<float[]> layerWeights = new List<float[]>();

            int previousNeurons = nnStructure[i - 1];

            //Weights layer
            for (int j = 0; j < nnStructure[i]; j++)
            {
                float[] weightsConnections = new float[previousNeurons];

                for (int k = 0; k < previousNeurons; k++)
                {
                    weightsConnections[k] = weightsCopy[i - 1][j][k];
                }

                layerWeights.Add(weightsConnections);
            }
            tempWeights.Add(layerWeights.ToArray());
        }

        weights = tempWeights.ToArray();
    }


    //Forward Feed takes in a set of inputs and returns a set of outputs
    public float[] forwardFeed(float[] inputs)
    {

        //we will translate the inputs into the neural net
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //Interate through the rest of the neurons 
        //For the number of layers in the NN loop through
        for (int i = 1; i < nnStructure.Length; i++)
        {
            //For every neuron in the layer, we start at the second layer
            for (int neuronIndex = 0; neuronIndex < nnStructure[i]; neuronIndex++)
            {
                float totalWeight = 0f;
                //for every previous neuron that's connected to the current neuron
                for (int prevNeuronIndex = 0; prevNeuronIndex < nnStructure[i - 1]; prevNeuronIndex++)
                {
                    totalWeight += weights[i - 1][neuronIndex][prevNeuronIndex] * neurons[i - 1][prevNeuronIndex];
                }

                neurons[i][neuronIndex] = (float)System.Math.Tanh(totalWeight);
            }
        }

        return neurons[nnStructure.Length - 1];
    }

    //All weights have a low chance of mutation when this method is called
    public void mutate(int mutationRate)
    {
        for (int layer = 0; layer < weights.Length; layer++)
        {
            for (int neuron = 0; neuron < weights[layer].Length; neuron++)
            {
                for (int weight = 0; weight < weights[layer][neuron].Length; weight++)
                {
                    float mutatedWeight = weights[layer][neuron][weight];

                    int randomValue = UnityEngine.Random.Range(0, mutationRate);

                    if (randomValue <= 2)
                    {
                        mutatedWeight = mutatedWeight * -1;
                    }
                    else if (randomValue <= 4)
                    {
                        mutatedWeight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomValue <= 6)
                    {
                        mutatedWeight = mutatedWeight * (UnityEngine.Random.Range(0f, 1f) + 1f);
                    }
                    else if (randomValue <= 8)
                    {
                        mutatedWeight = mutatedWeight * UnityEngine.Random.Range(0, 1);
                    }

                    weights[layer][neuron][weight] = mutatedWeight;
                }
            }
        }
    }

    public NeuralNet crossOver(NeuralNet partnerNeuralNet)
    {
        NeuralNet offSpring = new NeuralNet(nnStructure, this.weights, partnerNeuralNet.weights);
        return offSpring;
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

    public int[] getStructure()
    {
        return nnStructure;
    }

    public float[][][] getWeights()
    {
        return weights;
    }

    //Serialize 3D array of weights
    public float[] flattenWeightsToArray()
    {
        int sizeOfFlattenedArray = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                sizeOfFlattenedArray += weights[i][j].Length;
            }
        }

        float[] weightsFlattened = new float[sizeOfFlattenedArray];
        int indexer = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weightsFlattened[indexer] = weights[i][j][k];
                    indexer++;
                }
            }
        }
        return weightsFlattened;
    }

    public int getGeneration()
    {
        return generation;
    }
}

