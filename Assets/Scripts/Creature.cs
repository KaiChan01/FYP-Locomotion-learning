/* A base script to run the algorithm and simulate the creature
 * Author: Ka Yu Chan
 * Date: 11/10/2017
 */

using System.Collections;
using UnityEngine;

public class Creature : MonoBehaviour {

    public GameObject firstJoint;
    public GameObject secondJoint;
    public GameObject target;


    private Vector3 creatureStartPosition;
    private Vector3 creatureStartPosition2;

    private Quaternion creatureStartRotation;
    private Quaternion creatureStartRotation2;

    private Vector3 targetPosition;
    private System.Random randomGenerator = new System.Random();
    private int populationSize = 10;
    private int geneLoopCount = 5;
    private int geneSize = 5;
    private float mutationRate = 0.05f;
    private float startingDist;
    private bool nextMovement;

    private GeneticAlgorithm geneticAlgorithm;

    private HingeJoint rotationPoint;
    private HingeJoint startJointSetting;
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

        creatureStartPosition = firstJoint.transform.position;
        creatureStartPosition2 = secondJoint.transform.position;
        creatureStartRotation = firstJoint.transform.rotation;
        creatureStartRotation2 = secondJoint.transform.rotation;
        targetPosition = target.transform.position;

        startingDist = Vector3.Distance(creatureStartPosition, targetPosition);

        geneticAlgorithm = new GeneticAlgorithm(populationSize, geneSize, randomGenerator, mutationRate, startingDist, targetPosition);
        geneticAlgorithm.populate();
        Time.timeScale = 1;

        nextMovement = true;

        enabled = true;
        Application.runInBackground = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (nextMovement)
        {
            if (geneticAlgorithm.generatation == 10)
            {
                geneLoopCount = 20;
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 1;
                geneLoopCount = 5;
            }
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
                        {
                            GeneType genes = geneticAlgorithm.population[i].genes[j];
                            jLimit.max = genes.jLimitMax;
                            jLimit.min = genes.jLimitMax;
                            motor.targetVelocity = genes.targetVelocity;
                            motor.force = 500;
                            //rotationPoint.axis = genes.newAxis;
                            rotationPoint.limits = jLimit;
                            rotationPoint.motor = motor;
                            rotationPoint.useMotor = true;
                            nextMovement = false;
                            yield return new WaitForSeconds(2);
                            nextMovement = true;
                        }
                    }
                    counter++;
                }

                geneticAlgorithm.population[i].calculateFitness(firstJoint.transform.position);
                //resets too much for some reason
                resetCreature();
            }

            geneticAlgorithm.calculateTotalFitness(startingDist);
            geneticAlgorithm.breedNewGeneration();
            print(geneticAlgorithm.generatation);
    }
}


