using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreature : MonoBehaviour {

    NeuralNet forwardNet;
    NeuralNet backwardNet;
    NeuralNet leftNet;
    NeuralNet rightNet;
    NeuralNet idleNet;

    public GameObject mainBody;
    public GameObject[] jointObjects;
    private Limb[] limbs;
    private float spawnHeight;

    bool keyDown;

    // Use this for initialization
    void Start () {
        //create limb objects
        limbs = new Limb[jointObjects.Length];
        for (int i = 0; i < jointObjects.Length; i++)
        {
            limbs[i] = new Limb(jointObjects[i], mainBody);
        }

        keyDown = false;
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        keyDown = false;

        List<float> inputs = new List<float>();
        for (int i = 0; i < limbs.Length; i++)
        {
            for (int j = 0; j < limbs[i].getJointNum(); j++)
            {
                inputs.Add(limbs[i].getJointAngle(j));
            }
        }

        float[] inputsArray = inputs.ToArray();

        /*
        if (Input.anyKeyDown)
        {
            keyDown = true;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        }
        else if (!Input.anyKey && keyDown)
        {
            keyDown = false;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        }
        */

        if (Input.GetKey(KeyCode.A))
        {
            float[] outputs = leftNet.forwardFeed(inputsArray);
            mapOutputsToInstruction(outputs);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            float[] outputs = forwardNet.forwardFeed(inputsArray);
            mapOutputsToInstruction(outputs);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            float[] outputs = backwardNet.forwardFeed(inputsArray);
            mapOutputsToInstruction(outputs);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float[] outputs = rightNet.forwardFeed(inputsArray);
            mapOutputsToInstruction(outputs);
        }
        else if (Input.anyKey == false)
        {
            for(int i = 0; i < limbs.Length; i++)
            {
                limbs[i].revertToOriginalPosition();
            }
        }

        
    }

    public void mapOutputsToInstruction(float[] outputs)
    {
        int outputIndex = 0;
        for (int i = 0; i < limbs.Length; i++)
        {
            for (int j = 0; j < limbs[i].getJointNum(); j++)
            {
                limbs[i].addForceToHinge(outputs[outputIndex], j);
                outputIndex++;
            }
        }
    }

    public void setForwardNetwork(NeuralNet forwardNet)
    {
        this.forwardNet = forwardNet;
    }

    public void setBackwardNetwork(NeuralNet backwardNet)
    {
        this.backwardNet = backwardNet;
    }

    public void setLeftNetwork(NeuralNet leftNet)
    {
        this.leftNet = leftNet;
    }

    public void setRightNetwork(NeuralNet rightNet)
    {
        this.rightNet = rightNet;
    }

}
