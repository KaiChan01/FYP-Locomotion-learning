using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject body { get; private set; }
    public GameObject frontLeftT { get; private set; }
    public GameObject frontRightT { get; private set; }
    public GameObject frontLeftL { get; private set; }
    public GameObject frontRightL { get; private set; }
    public GameObject backLeftT { get; private set; }
    public GameObject backRightT { get; private set; }
    public GameObject backLeftL { get; private set; }
    public GameObject backRightL { get; private set; }

    //There's 4 limbs in this creature we're testing
    public Limb rightArm { get; private set; }
    public Limb leftArm { get; private set; }
    public Limb rightLeg { get; private set; }
    public Limb leftLeg { get; private set; }

    private NeuralNet brain = null;
    private Transform creatureStartTransform;
    private float fitness;
    private bool brainAssigned;
    private bool finishedInit = false;


    // Use this for initialization
    void Start () {
        body = this.transform.Find("Body").gameObject;
        frontLeftT = this.transform.Find("Front left thigh").gameObject;
        frontRightT = this.transform.Find("Front right thigh").gameObject;
        frontLeftL = this.transform.Find("Front left leg").gameObject;
        frontRightL = this.transform.Find("Front right leg").gameObject;
        backLeftT = this.transform.Find("Back left thigh").gameObject;
        backRightT = this.transform.Find("Back right thigh").gameObject;
        backLeftL = this.transform.Find("Back left leg").gameObject;
        backRightL = this.transform.Find("Back right leg").gameObject;

        rightArm = new Limb(frontRightT, body);
        leftArm = new Limb(frontLeftT, body);
        rightLeg = new Limb(backRightT, body);
        leftLeg = new Limb(backLeftT, body);

        this.creatureStartTransform = this.transform;
        //this.training = false;
        fitness = 0;
        finishedInit = true;
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
	}

    /*
    public void resetPosition()
    {
        this.transform.position = creatureStartTransform.position;
        this.transform.rotation = creatureStartTransform.rotation;
        resetFitness();
    }
    */

    public void mapOutputsToInstruction(float[] outputs)
    {
        //I need to come up with a better way of doing this, maybe with Public variables on the script

        //Debug.Log(outputs[0]);

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
        fitness = Vector3.Distance(creatureStartTransform.transform.position, new Vector3(creatureStartTransform.transform.position.x, creatureStartTransform.transform.position.y, this.transform.position.z));
    }

    public float getFitness()
    {
        return fitness;
    }

    /*
    public void resetFitness()
    {
        fitness = 0;
    }
    */

    public void setBrain(NeuralNet newBrain)
    {
        brain = newBrain;
    }

    /*
    public void assignedBrain()
    {
        brainAssigned = true;
        Debug.Log("run");
        Debug.Log(brainAssigned);
    }
    */
}
