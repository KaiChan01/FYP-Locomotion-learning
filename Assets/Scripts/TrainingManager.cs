using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

    private int numberOfCreatures = 10;
    public GameObject creaturePrefab;
    private Creature[] creaturesArray;
    private bool training;

    //Not sure how to determine the layout of the neural net yet
    private int[] neuralNetLayout = { 8, 8, 8, 8, 8, 8 };

    GeneticAlgorithm ga;

	// Use this for initialization
	void Start () {
        training = false;
        creaturesArray = new Creature[numberOfCreatures];
        ga = new GeneticAlgorithm(numberOfCreatures, neuralNetLayout);
        ga.populate();
        putCreaturesInScene();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void putCreaturesInScene()
    {
        for(int i = 0; i < numberOfCreatures; i++)
        {
            creaturesArray[i] = ((GameObject)Instantiate(creaturePrefab, new Vector3(UnityEngine.Random.Range(-50f, 50f), -0.5, UnityEngine.Random.Range(-50f, 50f)), creaturePrefab.transform.rotation)).GetComponent<Creature>();
            creaturesArray[i].setBrain(ga.population[i]);
        }
    }
}
