using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    //Variables for the type of locomotion to be trained
    public TrainingType trainingType;
    private Vector3 target;
    private Vector3 startingPos;
    private Vector3 previousPos;
    private float startingDistanceToTarget;

    //Store limb and joints
    public GameObject mainBody;
    public GameObject[] jointObjects;
    private Limb[] limbs;

    //Configurable values for measuring fitness
    public float standingFitness;
    public float desiredSpeed;
    public float balanceThreshold;

    //Detects collison
    private BodyCollision bodycoll;

    private NeuralNet brain = null;
    private bool alive;

    private float fitness;
    private TextMesh statusDesc;
    private string statusString;
    
    private bool finishedInit = false;

    //Used for serialisation
    [HideInInspector]
    public bool training;
    [HideInInspector]
    public int generation;
    [HideInInspector]
    public int trainingTime;

    void Start () {
        statusDesc = GetComponent<TextMesh>();
        bodycoll = mainBody.GetComponent<BodyCollision>();

        //Create Limb objects that can control the joint movements
        limbs = new Limb[jointObjects.Length];
        for(int i = 0; i < jointObjects.Length; i++)
        {
            limbs[i] = new Limb(jointObjects[i]);
        }

        fitness = 0;
        alive = true;

        startingPos = mainBody.transform.position;
        previousPos = mainBody.transform.position;


        //Create target position for the creature based on selected training type
        switch (trainingType)
        {
            case TrainingType.forward:
                target = new Vector3(startingPos.x, startingPos.y, startingPos.z + 10000);
                break;

            case TrainingType.backward:
                target = new Vector3(startingPos.x, startingPos.y, startingPos.z - 10000);
                break;

            case TrainingType.left:
                target = new Vector3(startingPos.x - 10000, startingPos.y, startingPos.z);
                break;

            case TrainingType.right:
                target = new Vector3(startingPos.x + 10000, startingPos.y, startingPos.z);
                break;
        }

        startingDistanceToTarget = Vector3.Distance(startingPos, target);

        //Check if alive after the given training time has expired
        Invoke("checkIfAlive", trainingTime);

        //Finished initialising
        finishedInit = true;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //If the creature finished initializing
        if (finishedInit == true)
        {
            //Create an array of inputs from the angle of the limbs
            List<float> inputs = new List<float>();
            for(int i = 0; i < limbs.Length; i++)
            {
                for(int j = 0; j < limbs[i].getJointNum(); j++)
                {
                    inputs.Add(limbs[i].getJointAngle(j));
                }
            }
            float[] inputsArray = inputs.ToArray();

            //feed inputs into neural net to calculate output
            float[] outputs = brain.forwardFeed(inputsArray);

            //Move the limbs to calculated position
            mapOutputsToInstruction(outputs);
        }

        if (training && alive)
        {
            calculateFitness();
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


    public void calculateFitness()
    {
        statusString = "";

        Vector3 currentPosition = mainBody.transform.position;

        //Speed and distance
        float distanceToTarget = Vector3.Distance(target, currentPosition);
        this.fitness += startingDistanceToTarget - distanceToTarget;

        //Update textmesh
        statusString += "Target: " + target + "\n";
        statusString += "Position: " + currentPosition + "\n";
        statusString += "Distance to Target: " + distanceToTarget + "\n";

        //Body collision with floor
        if (bodycoll.isTouchingGround())
        {
            this.fitness -= 0.5f;
            statusString += "BodyTouchingFloor: True\n";
        }
        else
        {
            this.fitness += 0.1f;
            statusString += "BodyTouchingFloor: False\n";
        }

        //Balance
        float angleX = convertAngle(mainBody.transform.rotation.eulerAngles.x);
        float angleY = convertAngle(mainBody.transform.rotation.eulerAngles.y);
        float angleZ = convertAngle(mainBody.transform.rotation.eulerAngles.z);

        // Calculate the level of imbalnce of the creature, deduct fitness if threshold is met
        float imbalanceMeasure = (angleX + angleY + angleZ) * 0.1f; ;

        if (imbalanceMeasure < balanceThreshold)
        {
            this.fitness += 1;
        }
        else
        {
            this.fitness -= imbalanceMeasure;
        }

        statusString += "ImbalanceLevel: "+ imbalanceMeasure + "\n";

        //Check if creature is standing as tall as desired
        if (currentPosition.y <= standingFitness)
        {
            this.fitness -= 0.3f;
            statusString += "Standing: false ("+ currentPosition.y+")\n";
        }
        else
        {
            this.fitness += currentPosition.y;
            statusString += "Standing: true(" + currentPosition.y + ")\n";
        }

        // For calculating distance travelled to target
        if(trainingType == TrainingType.forward || trainingType == TrainingType.backward)
        {
            this.fitness -= Mathf.Abs(currentPosition.x - startingPos.x);
        }
        else if (trainingType == TrainingType.left || trainingType == TrainingType.right)
        {
            this.fitness -= Mathf.Abs(currentPosition.z - startingPos.z);
        }

        statusString += "Fitness: " + fitness + "\n";


        updateTextMesh();
        brain.setFitness(fitness);
    }

    private void updateTextMesh()
    {
        statusDesc.text = statusString;
    }

    public float getFitness()
    {
        return fitness;
    }

    public bool getAlive()
    {
        return alive;
    }

    public void setBrain(NeuralNet newBrain)
    {
        brain = newBrain;
    }

    private float convertAngle(float angle)
    {
        float newAngle = angle % 360;
        if (newAngle > 270)
        {
            newAngle = newAngle - 360;
        }

        return Mathf.Abs(newAngle);
    }


    // method to check if creature is still active
    private void checkIfAlive()
    {
        // If the creature isn't moving fast enough it is deemed dead
        float distanceTravelled = Vector3.Distance(previousPos, mainBody.transform.position);
        if(distanceTravelled < desiredSpeed || fitness < 1000)
        {
            alive = false;
        }
        previousPos = mainBody.transform.position;
        Invoke("checkIfAlive", 1);
    }
}
