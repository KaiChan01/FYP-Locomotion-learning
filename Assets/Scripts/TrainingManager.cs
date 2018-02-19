using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour {

    private int numberOfCreatures = 10;
    private int numberOfParent = 5;
    public GameObject creaturePrefab;
    private List<Creature> creatureList;
    private bool training;

    public Text fitnessDisplay;

    //private int setTrainingTime = 10;
    //private float timer;

    //Not sure how to determine the layout of the neural net yet
    private int[] neuralNetLayout = { 8, 16, 16, 16, 16, 16,16, 8 };

    GeneticAlgorithm ga;

	// Use this for initialization
	void Start () {
        training = false;
        ga = new GeneticAlgorithm(numberOfCreatures, neuralNetLayout);
        ga.populate();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKey("down") && Time.timeScale > 0)
        {

            Time.timeScale = 10;
        }

        if (Input.GetKey("up") && Time.timeScale < 101)
        {
            Time.timeScale = 100;
        }

        if (Input.GetKey("left"))
        {
            Time.timeScale = 1;
        }

        if (training == true)
        {
            updateFitnessDisplay();
        }
        else
        {
            prepNextGeneration();
            training = true;
            Debug.Log(ga.generatation);
            //timer = setTrainingTime;
            Invoke("stopTraining", 10);
            resetFitnessDisplay();
        }
	}

    public void stopTraining()
    {
        training = false;
        for(int i = 0; i< numberOfCreatures; i++)
        {
            creatureList[i].calculateFitness();
            ga.population[i].setFitness(creatureList[i].getFitness());
        }
    }

    public void prepNextGeneration()
    {
        if(creatureList != null)
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                GameObject.Destroy(creatureList[i].gameObject);
            }
        }

        if(ga.generatation > 0)
        {
            ga.replaceOldParentWithNew(numberOfParent);
        }

        ga.mutatePopulation();
        ga.incrementGeneration();

        putNewCreaturesInScene();
    }

    public void putNewCreaturesInScene()
    {
        creatureList = new List<Creature>();

        for (int i = 0; i < numberOfCreatures; i++)
        {
            creatureList.Add(((GameObject)Instantiate(creaturePrefab, new Vector3(UnityEngine.Random.Range(-50f, 50f), -1.5f, UnityEngine.Random.Range(-50f, 50f)), creaturePrefab.transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(ga.population[i]);
            //creatureList[i].assignedBrain();
            //Debug.Log(creatureList.brain)
        }
    }

    public void updateFitnessDisplay()
    {
        resetFitnessDisplay();
        for (int i = 0; i < numberOfCreatures; i++)
        {
            fitnessDisplay.text = fitnessDisplay.text + "Creature"+i+" fitness:" + ga.population[i].getFitness() + "\n";
        }
    }

    public void resetFitnessDisplay()
    {
        fitnessDisplay.text = "";
    }
}
