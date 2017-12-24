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
    private bool training;


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
        this.training = false;
    }
	
	// Update is called once per frame
	void Update () {

        if(training == true)
        {
            float[] inputs = {rightArm.getBodyConnAngle(), rightArm.getLegConnAngle(),
                leftArm.getBodyConnAngle(), leftArm.getLegConnAngle(),
                rightLeg.getBodyConnAngle(), rightLeg.getLegConnAngle(),
                leftLeg.getBodyConnAngle(), leftLeg.getLegConnAngle() };

            float[] outputs = brain.forwardFeed(inputs);
        }
	}

    public void resetPosition()
    {
        this.transform.position = creatureStartTransform.position;
        this.transform.rotation = creatureStartTransform.rotation;
        resetFitness();
    }

    public void trainingMethod()
    {
    }

    public void calculateFitness()
    {

    }

    public void resetFitness()
    {
        fitness = 0;
    }

    public void setBrain(NeuralNet newBrain)
    {
        this.brain = newBrain;
    }

    public void flipTraining()
    {
        this.training = !this.training;
    }
}
