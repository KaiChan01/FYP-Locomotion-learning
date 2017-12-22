

using System.Collections.Generic;

public class NeuralNet {

    /*
     * A neural net must have
     * 1. Inputs (8 inputs the joints)
     * 2. hidden layers
     * 3. Outputs (8 outputs the joints)
     */

    //list of float[] represent the layers and neurons
    int[] nnStructure;
    float[][] neurons;
    float[][][] weights;

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
        return null;
    }
}
