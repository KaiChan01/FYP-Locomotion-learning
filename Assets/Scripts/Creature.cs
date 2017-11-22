/* A base script to run the algorithm and simulate the creature
 * Author: Ka Yu Chan
 * Date: 11/10/2017
 */

using System.Collections;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject firstJoint;
    public GameObject secondJoint;
    public GameObject thirdJoint;
    public GameObject target;


    private Vector3 creatureStartPosition;
    private Vector3 creatureStartPosition2;
    private Vector3 creatureStartPosition3;
    private Quaternion creatureStartRotation;
    private Quaternion creatureStartRotation2;
    private Quaternion creatureStartRotation3;

    private Vector3 targetPosition;
    private System.Random randomGenerator = new System.Random(1);
    private int populationSize = 10;
    private int geneLoopCount = 5;
    private int geneSize = 10;
    private float startingDist;
    private float mutationRate = 0.02f;
    private bool nextMovement;

    private GeneticAlgorithm geneticAlgorithm;

    private HingeJoint rotationPoint;
    private HingeJoint rotationPoint2;
    private HingeJoint startJointSetting;
    private HingeJoint startJointSetting2;
    private JointMotor motor;
    private JointLimits jLimit;

    //private int numberofJoints = 2;
    // Use this for initialization
    void Start()
    {
        enabled = false;
        startJointSetting = rotationPoint = secondJoint.GetComponent<HingeJoint>();

        motor = rotationPoint.motor;
        jLimit = rotationPoint.limits;
        rotationPoint.useMotor = true;
        rotationPoint.useLimits = true;

        creatureStartPosition = firstJoint.transform.position;
        creatureStartPosition2 = secondJoint.transform.position;
        creatureStartRotation = firstJoint.transform.rotation;
        creatureStartRotation2 = secondJoint.transform.rotation;
        targetPosition = target.transform.position;

        startingDist = Vector3.Distance(creatureStartPosition, targetPosition);

        geneticAlgorithm = new GeneticAlgorithm(populationSize, randomGenerator, geneSize, startingDist, targetPosition);
        geneticAlgorithm.populate();
        Time.timeScale = 100;

        nextMovement = true;
        Application.runInBackground = true;
        enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("down") && Time.timeScale > 1)
        {

            Time.timeScale = 10;
        }

        if (Input.GetKey("up") && Time.timeScale < 100)
        {
            Time.timeScale = 100;
        }

        if (nextMovement)
        {
            

            StartCoroutine(moveLimb());
        }
        
    }

    public void resetCreature()
    {
        firstJoint.transform.position = creatureStartPosition;
        secondJoint.transform.position = creatureStartPosition2;
        firstJoint.transform.rotation = creatureStartRotation;
        secondJoint.transform.rotation = creatureStartRotation2;
        rotationPoint = startJointSetting;
    }

    IEnumerator moveLimb()
    {
        //Check fitness
        for (int i = 0; i < populationSize; i++)
        {
            int counter = 0;
            while (counter < geneLoopCount)
            {
                for (int j = 0; j < geneSize; j++)
                {
                    mapGeneToMovement(geneticAlgorithm.population[i].genes[j]);
                    motor.force = 500;
                    rotationPoint.motor = motor;
                    nextMovement = false;
                    yield return new WaitForSeconds(1);
                    nextMovement = true;
                }
                counter++;
            }

        geneticAlgorithm.population[i].calculateFitness(firstJoint.transform.position, targetPosition);
        Debug.Log("index: [" + i + "] " + geneticAlgorithm.population[i].fitnessValue);
        resetCreature();
        }

        //geneticAlgorithm.calculateTotalFitness(startingDist);
        geneticAlgorithm.breedNewGeneration(mutationRate);
        print("Generation: " + geneticAlgorithm.generatation);
    }

    private void mapGeneToMovement(int gene)
    {
        switch (gene)
        {
            //Contract limb
            case 0:
            {
                motor.targetVelocity = -200;
                break;
            }
            
            // extend limb
            case 1:
            {
                    motor.targetVelocity = 200;
                    break;
            }

            //turn left
            case 2:
            {
                    rotationPoint.axis = new Vector3(1, 0.1f, 0);
            }
                break;

            //turn right
            case 3:
            {
                    rotationPoint.axis = new Vector3(1, -0.1f, 0);
                    break;
            }
        }
    }
}


