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

    private NeuralNet brain = null;
    private Vector3 creatureStartPosition;
    private Vector3 target;
    private float fitness;
    private bool brainAssigned;
    private bool finishedInit = false;


    // Use this for initialization
    void Start () {

        rightArm = new Limb(frontRightT, body);
        leftArm = new Limb(frontLeftT, body);
        rightLeg = new Limb(backRightT, body);
        leftLeg = new Limb(backLeftT, body);

        //The body's position will calculate the fitness
        this.creatureStartPosition =  new Vector3(body.transform.position.x, body.transform.position.y, body.transform.position.z);
        this.target = new Vector3(creatureStartPosition.x, creatureStartPosition.y, 100);

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

        calculateFitness();

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
        //fitness = Vector3.Distance(creatureStartPosition, body.transform.position);
        //fitness = Vector3.Distance(creatureStartPosition, new Vector3(creatureStartPosition.x, creatureStartPosition.y, body.transform.position.z));
        fitness = 100 - Vector3.Distance(body.transform.position, target);
        brain.setFitness(fitness);
        //Debug.Log(fitness);
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
