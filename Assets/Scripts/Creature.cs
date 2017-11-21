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
    private int geneSize = 3;
    private float startingDist;
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
        //startJointSetting2 = rotationPoint2 = thirdJoint.GetComponent<HingeJoint>();

        motor = rotationPoint.motor;
        jLimit = rotationPoint.limits;

        creatureStartPosition = firstJoint.transform.position;
        creatureStartPosition2 = secondJoint.transform.position;
        //creatureStartPosition3 = thirdJoint.transform.position;
        creatureStartRotation = firstJoint.transform.rotation;
        creatureStartRotation2 = secondJoint.transform.rotation;
        //creatureStartRotation3 = thirdJoint.transform.rotation;
        targetPosition = target.transform.position;

        startingDist = Vector3.Distance(creatureStartPosition, targetPosition);

        geneticAlgorithm = new GeneticAlgorithm(populationSize, geneSize, startingDist, targetPosition);
        geneticAlgorithm.populate();
        Time.timeScale = 100;

        nextMovement = true;
        Application.runInBackground = true;
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextMovement)
        {
            if ((geneticAlgorithm.generatation%10) == 0)
            {

                Time.timeScale = 1;
                geneLoopCount = 15;
                StartCoroutine(viewFittest());
            }
            else
            {
                Time.timeScale = 100;
                geneLoopCount = 15;
                //training
                StartCoroutine(moveLimb());
            }
            
        }
        
    }

    public void resetCreature()
    {
        firstJoint.transform.position = creatureStartPosition;
        secondJoint.transform.position = creatureStartPosition2;
        //thirdJoint.transform.position = creatureStartPosition3;
        firstJoint.transform.rotation = creatureStartRotation;
        secondJoint.transform.rotation = creatureStartRotation2;
        //thirdJoint.transform.rotation = creatureStartRotation3;
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
                            rotationPoint.axis = genes.newAxis;
                            rotationPoint.motor = motor;
                            rotationPoint.useMotor = true;
                            nextMovement = false;
                            yield return new WaitForSeconds(1);
                            nextMovement = true;
                        }
                    }
                    counter++;
                }

            geneticAlgorithm.population[i].calculateFitness(firstJoint.transform.position);
            Debug.Log("index: [" + i + "] " + geneticAlgorithm.population[i].fitnessValue);
            resetCreature();
            }

        //geneticAlgorithm.calculateTotalFitness(startingDist);
        geneticAlgorithm.chooseBaseForNextGeneration();
        //geneticAlgorithm.mutatePopulation(randomGenerator);
        print(geneticAlgorithm.generatation);
    }

    IEnumerator viewFittest()
    {
        //Check fitness

            int counter = 0;
            while (counter < geneLoopCount)
            {
                for (int j = 0; j < geneSize; j++)
                {
                    {
                        GeneType genes = geneticAlgorithm.fittestSoFar.genes[j];
                        jLimit.max = genes.jLimitMax;
                        jLimit.min = genes.jLimitMax;
                        motor.targetVelocity = genes.targetVelocity;
                        motor.force = 500;
                        rotationPoint.axis = genes.newAxis;
                        rotationPoint.motor = motor;
                        rotationPoint.useMotor = true;
                        nextMovement = false;
                        yield return new WaitForSeconds(1);
                        nextMovement = true;
                    }
                }
                counter++;
            }
        resetCreature();
        geneticAlgorithm.generatation++;
        }
    }


