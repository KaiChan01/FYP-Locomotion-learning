using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    public TrainingType trainingType;
    private Vector3 target;
    private float startingDistanceToTarget;

    public GameObject mainBody;
    public GameObject[] jointObjects;

    public float standingFitness;
    public float desiredSpeed;
    private Limb[] limbs;

    private BodyCollision bodycoll;
    private NeuralNet brain = null;
    private Vector3 startingPos;
    private float fitness;
    private bool alive;
    private bool finishedInit = false;

    private TextMesh statusDesc;
    private string statusString;
    private Vector3 previousPos;

    [HideInInspector]
    public bool training;
    [HideInInspector]
    public int generation;
    [HideInInspector]
    public int trainingTime;

    void Start () {
        limbs = new Limb[jointObjects.Length];
        for(int i = 0; i < jointObjects.Length; i++)
        {
            limbs[i] = new Limb(jointObjects[i], mainBody);
        }

        fitness = 0;
        finishedInit = true;
        bodycoll = mainBody.GetComponent<BodyCollision>();
        statusDesc = GetComponent<TextMesh>();
        startingPos = mainBody.transform.position;

        alive = true;
        previousPos = mainBody.transform.position;
        Invoke("checkIfAlive", trainingTime);

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

            case TrainingType.standing:
                target = startingPos;
                break;
        }

        startingDistanceToTarget = Vector3.Distance(startingPos, target);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Right now the inputs are just the the angle's of the creature's joints

        if (finishedInit == true)
        {
            List<float> inputs = new List<float>();
            for(int i = 0; i < limbs.Length; i++)
            {
                for(int j = 0; j < limbs[i].getJointNum(); j++)
                {
                    inputs.Add(limbs[i].getJointAngle(j));
                }
            }
            float[] inputsArray = inputs.ToArray();

            float[] outputs = brain.forwardFeed(inputsArray);

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
        Vector3 currentPosition = mainBody.transform.position;

        statusString = "";

        //Speed and distance
        float distanceToTarget = Vector3.Distance(target, currentPosition);
        this.fitness += startingDistanceToTarget - distanceToTarget;

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

        // Use Euler angles
        float imbalanceMeasure = (angleX + angleY + angleZ) * 0.1f; ;

        if (imbalanceMeasure < 5)
        {
            this.fitness += 1;
        }
        else
        {
            this.fitness -= imbalanceMeasure;
        }

        statusString += "ImbalanceLevel: "+ imbalanceMeasure + "\n";

        //inverse distance

        //Standing fitness
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

    void checkIfAlive()
    {
        float distanceTravelled = Vector3.Distance(previousPos, mainBody.transform.position);
        if(distanceTravelled < desiredSpeed || fitness < 1000)
        {
            alive = false;
        }
        previousPos = mainBody.transform.position;
        Invoke("checkIfAlive", 1);
    }
}
