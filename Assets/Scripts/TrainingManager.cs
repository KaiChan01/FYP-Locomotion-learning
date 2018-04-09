using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TrainingType
{   forward = 0,
    backward = 1,
    left = 2,
    right = 3,
};

public class TrainingManager : MonoBehaviour {

    public GameObject creaturePrefab;
    private List<Creature> creatureList;
    public TrainingType trainingType;
    public int numberOfCreatures;
    public int numberOfParent;
    public Text fitnessDisplay;
    public int trainingTime;
    public int maxTimeLimit = 60;
    public int generationLimit;
    private int randomPhase;
    public int mutationRate;
    public float spawnHeight;
    public string trainingName;
    public bool saveTraining;
    private bool creaturesAlive;
    private float timePassedSinceNewGeneration;
    private bool training;
    private string creatureName;

    //Predetermined Neural Network structure
    public int[] neuralNetLayout = { 8, 8, 8, 8, 8, 8 };
    private string phase;

    GeneticAlgorithm ga;

    // Initialization
    void Start() {
        creatureName = creaturePrefab.name;
        randomPhase = numberOfCreatures;
        training = false;
        ga = new GeneticAlgorithm(numberOfCreatures, neuralNetLayout, generationLimit, mutationRate, trainingName, creatureName, spawnHeight);
        phase = "Initialising";
    }

    // Update once per frame
    void FixedUpdate() {

        //Controls for speeding up the simulation
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

        //Training and scene construction
        if (training == true)
        {
            updateFitnessDisplay();
        }
        else
        {
            prepNextGeneration();
            if (ga.generatation <= ga.generationLimit)
            {
                resetFitnessDisplay();
            }
            training = true;
            Debug.Log(ga.generatation);
        }

        //Stop traing when max time limit is met
        timePassedSinceNewGeneration += Time.deltaTime;
        if(timePassedSinceNewGeneration > maxTimeLimit)
        {
            stopTraining();
        }
    }

    //Stop training and calculate final fitness 
    public void stopTraining()
    {
        training = false;
        for (int i = 0; i < numberOfCreatures; i++)
        {
            creatureList[i].calculateFitness();
            ga.population[i].setFitness(creatureList[i].getFitness());
        }
    }

    //Preparing next generation
    public void prepNextGeneration()
    {
        if(ga.generatation > 0)
        {
            ga.saveLog();
        }

        if (creatureList != null)
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                GameObject.Destroy(creatureList[i].gameObject);
            }
        }

        //Mutate and optimise phase
        if (ga.generatation > randomPhase && ga.generatation < generationLimit)
        {
            ga.replaceOldParentWithNew(numberOfParent);

            ga.mutatePopulation(numberOfParent);
            phase = "Mutating";

            if (ga.generatation % 50 == 0 && saveTraining)
            {
                ga.saveNetwork(ga.selectBestNeuralNetFromCurrentGen());
            }
        }
        // Last generation
        else if (ga.generatation == generationLimit)
        {
            ga.showcaseAndSaveBestParent();
            phase = "Showcasing";
        }
        // Run is over
        else if (ga.generatation > generationLimit)
        {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
        }
        // Compare best parents
        else if (ga.generatation == randomPhase)
        {
            ga.testAllInitialBest();
            phase = "Testing Best From Random";

            ga.saveNetwork(ga.selectBestNeuralNetFromCurrentGen());

        }
        // Make random population
        else if (ga.generatation > 0)
        {
            ga.addBestParent();
            ga.createNewGeneration();
            phase = "Testing Random";
        }
        Debug.Log(phase);

        spawnCreaturesInScene();
        ga.incrementGeneration();
    }

    public void spawnCreaturesInScene()
    {
        creatureList = new List<Creature>();

        for (int i = 0; i < numberOfCreatures; i++)
        {
            creatureList.Add(((GameObject)Instantiate(creaturePrefab, new Vector3(-(numberOfCreatures * 10) + i * 20, spawnHeight, 0), creaturePrefab.transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(ga.population[i]);
            creatureList[i].training = true;
            creatureList[i].trainingTime = trainingTime;
            creatureList[i].trainingType = trainingType;
        }
        creaturesAlive = true;
        Invoke("checkIfCreaturesAreAlive", trainingTime);
        timePassedSinceNewGeneration = 0;
    }

    public void updateFitnessDisplay()
    {
        resetFitnessDisplay();
        for (int i = 0; i < numberOfCreatures; i++)
        {
            fitnessDisplay.text = fitnessDisplay.text + " Alive: " + creatureList[i].getAlive() + ". Creature" + i + " fitness:" + ga.population[i].getFitness() + "\n";
        }
        fitnessDisplay.text = fitnessDisplay.text + "Mutation rate: " + ga.mutationRate + "\n";
        fitnessDisplay.text = fitnessDisplay.text + "Fittest from last generation: " + ga.highestFromGeneration + "\n";
    }

    public void resetFitnessDisplay()
    {
        fitnessDisplay.text = "";
    }

    //Checks if any creatures are still alive
    public void checkIfCreaturesAreAlive()
    {
        bool creatureChecker = false;
        for (int i = 0; i < creatureList.Count; i++)
        {
            //Check if there are still creatures alive
            if (creatureList[i].getAlive())
            {
                creatureChecker = true;
            }
        }
        if (!creatureChecker)
        {
            creaturesAlive = false;
            stopTraining();
        }

        //If some creatures are alive, check again every 1 second
        if (creaturesAlive)
        {
            Invoke("checkIfCreaturesAreAlive", 1);
        }
    }
}
