using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject mainBody;
    public GameObject[] jointObjects;
    //public GameObject frontLeftT;
    //public GameObject frontRightT;
    //public GameObject backLeftT;
    //public GameObject backRightT;

    //There's 4 limbs in this creature we're testing
    /*
    public Limb rightArm { get; private set; }
    public Limb leftArm { get; private set; }
    public Limb rightLeg { get; private set; }
    public Limb leftLeg { get; private set; }
    */

    public float standingFitness;
    private Limb[] limbs;

    private BodyCollision bodycoll;
    private NeuralNet brain = null;
    private Vector3 previousPosition;
    private float fitness;
    private bool brainAssigned;
    private bool finishedInit = false;

    private Rigidbody bodyRB;
    private TextMesh statusDesc;
    private string statusString;

    void Start () {
        limbs = new Limb[jointObjects.Length];
        for(int i = 0; i < jointObjects.Length; i++)
        {
            limbs[i] = new Limb(jointObjects[i], mainBody);
        }

        //this.training = false;
        fitness = 0;
        finishedInit = true;
        bodycoll = mainBody.GetComponent<BodyCollision>();
        bodyRB = mainBody.GetComponent<Rigidbody>();
        statusDesc = GetComponent<TextMesh>();
        previousPosition = mainBody.transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Right now the inputs are just the the angle's of the creature's joints

        if (finishedInit == true)
        {
            /*
                float[] inputs = {rightArm.getBodyConnAngle(), rightArm.getLegConnAngle(),
                leftArm.getBodyConnAngle(), leftArm.getLegConnAngle(),
                rightLeg.getBodyConnAngle(), rightLeg.getLegConnAngle(),
                leftLeg.getBodyConnAngle(), leftLeg.getLegConnAngle() }
            */

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

        calculateFitness();
    }

    public void mapOutputsToInstruction(float[] outputs)
    {
        /*
        rightArm.addForceToBodyHinge(outputs[0]);
        rightArm.addForceToLegHinge(outputs[1]);
        leftArm.addForceToBodyHinge(outputs[2]);
        leftArm.addForceToLegHinge(outputs[3]);
        rightLeg.addForceToBodyHinge(outputs[4]);
        rightLeg.addForceToLegHinge(outputs[5]);
        leftLeg.addForceToBodyHinge(outputs[6]);
        leftLeg.addForceToLegHinge(outputs[7]);
        */

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

        //Speed and distance
        float distanceTravelled = Vector3.Distance(previousPosition, mainBody.transform.position);
        this.fitness += distanceTravelled;
        statusString += "Distance: " + distanceTravelled + "\n";

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
        if (mainBody.transform.position.y <= standingFitness)
        {
            this.fitness -= 0.3f;
            statusString += "Standing: false ("+ mainBody.transform.position.y+")\n";
        }
        else
        {
            this.fitness += mainBody.transform.position.y;
            statusString += "Standing: true(" + mainBody.transform.position.y + ")\n";
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
}
