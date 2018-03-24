using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimulationManager : MonoBehaviour {

    [Tooltip("Filenames in TrainedNetworks folder")]
    public string[] fileNames;
    private string trainedFilePath = "/TrainedNetworks/";
    private List<Creature> creatureList;
    public GameObject creaturePrefab;
    private List<NeuralNet> networkList = new List<NeuralNet>();
    private NeuralNet[] networkArray;
    private int spawnHeight = -1;

    // Use this for initialization
    void Start () {
        loadTrainedNetworks();
        putNewCreaturesInScene();
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
            }
            else
            {
                Debug.Log("File cannot be found");
            }
        }
        networkArray = networkList.ToArray();
    }

    public void putNewCreaturesInScene()
    {
        creatureList = new List<Creature>();

        for (int i = 0; i < networkArray.Length; i++)
        {
            creatureList.Add(((GameObject)Instantiate(creaturePrefab, new Vector3(i*15, spawnHeight, 0), creaturePrefab.transform.rotation)).GetComponent<Creature>());
            creatureList[i].setBrain(networkArray[i]);
            creatureList[i].training = false;
            creatureList[i].generation = networkArray[i].getGeneration();
        }
    }
}
