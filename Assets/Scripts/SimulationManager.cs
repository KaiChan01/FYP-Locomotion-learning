using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SimulationManager : MonoBehaviour {

    public GameObject[] creatures;
    private string[] creatureNames; 

    [Tooltip("Filenames in TrainedNetworks folder")]
    public string[] fileNames;
    private string trainedFilePath = "/TrainedNetworks/";
    private List<string> creatureNamesToLoad = new List<string>();
    private List<Creature> creatureList = new List<Creature>();
    private List<float> spawnHeights = new List<float>();
    private List<NeuralNet> networkList = new List<NeuralNet>();
    private NeuralNet[] networkArray;

    // Use this for initialization
    void Start () {
        initializeCreatureNames();
        loadTrainedNetworks();
        spawnCreaturesInScene();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void loadTrainedNetworks()
    {
        for(int i = 0; i < fileNames.Length; i++)
        {
            string filePath = Application.dataPath + trainedFilePath + fileNames[i];
            TrainedNetwork trainedNetwork;
            if (File.Exists(filePath))
            {
                string neuralData = File.ReadAllText(filePath);
                trainedNetwork = JsonUtility.FromJson<TrainedNetwork>(neuralData);
                networkList.Add(new NeuralNet(trainedNetwork.weights, trainedNetwork.nnStructure, trainedNetwork.generation));
                creatureNamesToLoad.Add(trainedNetwork.creatureName);
                spawnHeights.Add(trainedNetwork.spawnHeight);
            }
            else
            {
                Debug.Log("File cannot be found");
            }
        }
        networkArray = networkList.ToArray();
    }

    public void spawnCreaturesInScene()
    {
        creatureList = new List<Creature>();

        for (int i = 0; i < networkArray.Length; i++)
        {
            string creatureType = creatureNamesToLoad[i];
            int prefabIndex = ArrayUtility.IndexOf(creatureNames, creatureType); 

            creatureList.Add(((GameObject)Instantiate(creatures[prefabIndex], new Vector3(i*15, spawnHeights[i], 0), creatures[prefabIndex].transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(networkArray[i]);
            creatureList[i].training = false;
            creatureList[i].generation = networkArray[i].getGeneration();
        }
    }

    public void initializeCreatureNames()
    {
        creatureNames = new string[creatures.Length];
        for (int i = 0; i < creatures.Length; i++)
        {
            creatureNames[i] = creatures[i].name;
        }
    }
}
