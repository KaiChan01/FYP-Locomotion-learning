﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

    private int numberOfCreatures = 10;
    public GameObject creaturePrefab;
    private List<Creature> creatureList;
    private bool training;

    //private int setTrainingTime = 10;
    //private float timer;

    //Not sure how to determine the layout of the neural net yet
    private int[] neuralNetLayout = { 8, 8, 8, 8, 8, 8 };

    GeneticAlgorithm ga;

	// Use this for initialization
	void Start () {
        training = false;
        creatureList = new List<Creature>();
        ga = new GeneticAlgorithm(numberOfCreatures, neuralNetLayout);
        ga.populate();
    }
	
	// Update is called once per frame
	void Update () {

		if(training == true)
        {
            Invoke("stopTraining", 10);
        }
        else
        {
            prepNextGeneration();
            training = true;
            //timer = setTrainingTime;
        }
	}

    public void stopTraining()
    {
        training = false;
        for(int i = 0; i< numberOfCreatures; i++)
        {
            ga.population[i].setFitness(creatureList[i].getFitness());
        }
    }

    public void prepNextGeneration()
    {
        /*
        for(int i = 0; i < creatureList.Count; i++)
        {
            GameObject.Destroy(creatureList[i].gameObject);
        }
        */

        while(creatureList.Count != 0)
        {
            GameObject.Destroy(creatureList[0].gameObject);
        }


        ga.mutatePopulation();
        ga.incrementGeneration();

        putNewCreaturesInScene();
    }

    public void putNewCreaturesInScene()
    {
        for(int i = 0; i < numberOfCreatures; i++)
        {
            creatureList.Add(((GameObject)Instantiate(creaturePrefab, new Vector3(UnityEngine.Random.Range(-50f, 50f), -0.5f, UnityEngine.Random.Range(-50f, 50f)), creaturePrefab.transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(ga.population[i]);
            creatureList[i].assignedBrain();
        }
    }
}
