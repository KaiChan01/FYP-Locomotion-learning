using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerSpawner : MonoBehaviour {

    [Tooltip("Enter the filenames for movement in order: forward, backwards, left, right and standing")]
    public string[] fileNames = new string[4];
    private string trainedFilePath = "/TrainedMovementNetworks/";
    public GameObject creaturePrefab;
    private List<NeuralNet> networkList = new List<NeuralNet>();
    private NeuralNet[] networkArray;
    public int spawnHeight;
    private PlayerCreature player;

    // Use this for initialization
    void Start () {
        loadTrainedNetworks();
        spawnPlayer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void loadTrainedNetworks()
    {
        for (int i = 0; i < fileNames.Length; i++)
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

    public void spawnPlayer()
    {
        player = ((GameObject)Instantiate(creaturePrefab, new Vector3(0, spawnHeight, 0), creaturePrefab.transform.rotation)).GetComponent<PlayerCreature>();
        player.setForwardNetwork(networkArray[0]);
        player.setBackwardNetwork(networkArray[1]);
        player.setLeftNetwork(networkArray[2]);
        player.setRightNetwork(networkArray[3]);
    }
}
