using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour {

    public int numberOfCreatures;
    public int numberOfParent;
    public GameObject creaturePrefab;
    private List<Creature> creatureList;
    private bool training;
    public Text fitnessDisplay;
    public int trainingTime;
    public int runLimit;
    public int randomPhase;
    public int mutationRate;

    //Not sure how to determine the layout of the neural net yet
    private int[] neuralNetLayout = { 8, 32, 8 };
    private string phase;

    GeneticAlgorithm ga;

	// Use this for initialization
	void Start () {
        training = false;
        ga = new GeneticAlgorithm(numberOfCreatures, neuralNetLayout, runLimit, randomPhase, mutationRate);
        phase = "Initialising";
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKey("right") && Time.timeScale > 0)
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
            if (ga.generatation < ga.runLimit)
            {
                Invoke("stopTraining", trainingTime);
                resetFitnessDisplay();
            }
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

        if(ga.generatation > randomPhase)
        {
            ga.replaceOldParentWithNew(numberOfParent);
            //ga.breedNewGeneration(numberOfParent);

            ga.mutatePopulation(numberOfParent);
            phase = "Mutating";
        }
        else if (ga.generatation == ga.runLimit)
        {
            ga.showcaseBestParent();
            phase = "Showcasing";
        }
        else if(ga.generatation > 0)
        {
            ga.addBestParent();
            ga.createRandomGeneration();
            phase = "Testing Random";
        }
        else if(ga.generatation == randomPhase)
        {
            ga.testAllInitialBest();
            phase = "Testing Best From Random";
        }

        Debug.Log(phase);

        putNewCreaturesInScene();
        ga.incrementGeneration();
    }

    public void putNewCreaturesInScene()
    {
        creatureList = new List<Creature>();

        for (int i = 0; i < numberOfCreatures; i++)
        {
            creatureList.Add(((GameObject)Instantiate(creaturePrefab, new Vector3(-(numberOfCreatures * 10) + i * 20, -1.5f, 0), creaturePrefab.transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(ga.population[i]);
        }
    }

    public void updateFitnessDisplay()
    {
        resetFitnessDisplay();
        for (int i = 0; i < numberOfCreatures; i++)
        {
            fitnessDisplay.text = fitnessDisplay.text + "Creature"+ i +" fitness:" + ga.population[i].getFitness() + "\n";
        }
        fitnessDisplay.text = fitnessDisplay.text + "Mutation rate: " + ga.mutationRate + "\n";
        fitnessDisplay.text = fitnessDisplay.text + "Fittest from last generation: " + ga.highestFromGeneration + "\n";
    }

    public void resetFitnessDisplay()
    {
        fitnessDisplay.text = "";
    }
}
