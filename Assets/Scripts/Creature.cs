using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject body;
    public GameObject frontLeftT;
    public GameObject frontRightT;
    public GameObject frontLeftL;
    public GameObject frontRightL;
    public GameObject backLeftT;
    public GameObject backRightT;
    public GameObject backLeftL;
    public GameObject backRightL;

    //There's 4 limbs in this creature we're testing
    public Limb rightArm { get; private set; }
    public Limb leftArm { get; private set; }
    public Limb rightLeg { get; private set; }
    public Limb leftLeg { get; private set; }

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

        rightArm = new Limb(frontRightT, body);
        leftArm = new Limb(frontLeftT, body);
        rightLeg = new Limb(backRightT, body);
        leftLeg = new Limb(backLeftT, body);

        //this.training = false;
        fitness = 0;
        finishedInit = true;
        bodycoll = body.GetComponent<BodyCollision>();
        bodyRB = body.GetComponent<Rigidbody>();
        statusDesc = GetComponent<TextMesh>();
        previousPosition = body.transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Right now the inputs are just the the angle's of the creature's joints

        if (finishedInit == true)
        {
            float[] inputs = {rightArm.getBodyConnAngle(), rightArm.getLegConnAngle(),
                leftArm.getBodyConnAngle(), leftArm.getLegConnAngle(),
                rightLeg.getBodyConnAngle(), rightLeg.getLegConnAngle(),
                leftLeg.getBodyConnAngle(), leftLeg.getLegConnAngle() };

            float[] outputs = brain.forwardFeed(inputs);

            mapOutputsToInstruction(outputs);
        }

        calculateFitness();
        previousPosition = transform.position;
    }

    public void mapOutputsToInstruction(float[] outputs)
    {
        rightArm.addForceToBodyHinge(outputs[0]);
        rightArm.addForceToLegHinge(outputs[1]);
        leftArm.addForceToBodyHinge(outputs[2]);
        leftArm.addForceToLegHinge(outputs[3]);
        rightLeg.addForceToBodyHinge(outputs[4]);
        rightLeg.addForceToLegHinge(outputs[5]);
        leftLeg.addForceToBodyHinge(outputs[6]);
        leftLeg.addForceToLegHinge(outputs[7]);
    }

    public void calculateFitness()
    {
        statusString = "";

        //Speed and distance
        float distanceTravelled = Vector3.Distance(previousPosition, body.transform.position);
        this.fitness += distanceTravelled;
        statusString += "MovingSpeed: " + distanceTravelled + "\n";

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
        float angleX = convertAngle(body.transform.rotation.eulerAngles.x);
        float angleY = convertAngle(body.transform.rotation.eulerAngles.y);
        float angleZ = convertAngle(body.transform.rotation.eulerAngles.z);

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
        if (body.transform.position.y <= 0.35f)
        {
            this.fitness -= 0.3f;
            statusString += "Standing: false\n";
        }
        else
        {
            this.fitness += body.transform.position.y;
            statusString += "Standing: true\n";
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
