using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftEvolutionManager : MonoBehaviour {
	public static bool stopAllMutation;

	public float mutaitonMag;

//	public float minMutationMag = 0.05f;
//	public float maxMutaitonMag = 1f;

	public int increaseStages = 20;
	public bool stopMutation;

	public NeuralGraphVisualizer graphVisualizer;

	NeuralGraph graph;

	public HoverCraftNeuralController neuralController;


	[Range (0, 500)] public int population = 20;

	[HideInInspector] public HoverCraftNeuralController [] neuralControllers;
	[HideInInspector] public Transform craftsParent;

	[HideInInspector] public float [] fitnessList;
	[HideInInspector] public float timeForFitnessCheck;

	[HideInInspector] public NeuralGraph currentBestGraph;
	[HideInInspector] public float currentBestFitness;

	void Start () {
		graph = graphVisualizer.GetNeuralGraph ();


		neuralController.gameObject.SetActive (false);

		craftsParent = new GameObject ().transform;
//		craftsParent.transform.SetParent (transform);
//		mutaitonMag = minMutationMag;

		neuralControllers = new HoverCraftNeuralController[population];
		fitnessList = new float[population];
		for (int i = 0; i < population; i++) {
			HoverCraftNeuralController neuralControllerCopy = Instantiate<GameObject> (neuralController.gameObject).GetComponent<HoverCraftNeuralController> ();
			neuralControllerCopy.transform.SetParent (craftsParent);
			neuralControllerCopy.SetNeuralGraph (graph, mutaitonMag);
			neuralControllerCopy.Reset ();
			neuralControllerCopy.gameObject.SetActive (true);
			neuralControllers [i] = neuralControllerCopy;
		}
		currentBestFitness = 0f;
		currentBestGraph = neuralControllers [0].neuralGraph;
	}

	void ResetPopulation () {
		HoverCraftNeuralController bestController = neuralControllers [0];
		bestController.fitness = currentBestFitness;

		for (int i = 1; i < neuralControllers.Length; i++) {
			if (neuralControllers [i].fitness > bestController.fitness) {
//				Debug.Log ("Beat! " + neuralControllers[i].fitness + ", oldBest: " + bestController.fitness);
				bestController = neuralControllers [i];
			}
		}
		if (bestController.fitness <= currentBestFitness) {
			bestController.neuralGraph = currentBestGraph;
			bestController.fitness = currentBestFitness;

//			float mutaitonDelta = (maxMutaitonMag - minMutationMag) / increaseStages;
//			mutaitonMag = Mathf.Clamp (mutaitonMag + mutaitonDelta, minMutationMag, maxMutaitonMag); 
		} else {
			currentBestGraph = bestController.neuralGraph;
			currentBestFitness = bestController.fitness;

//			mutaitonMag = minMutationMag;
		}

		fitnessList [0] = 0f;
		neuralControllers [0].SetNeuralGraph (bestController.neuralGraph, 0f);
		neuralControllers [0].neuralGraph = graph;
		neuralControllers [0].transform.position = neuralController.transform.position;
		neuralControllers [0].transform.eulerAngles = neuralController.transform.eulerAngles;
		neuralControllers [0].Reset ();

		for (int i = 1; i < neuralControllers.Length; i++) {
			fitnessList [i] = 0f;
			neuralControllers [i].SetNeuralGraph (bestController.neuralGraph, mutaitonMag);
			neuralControllers [i].transform.position = neuralController.transform.position;
			neuralControllers [i].transform.eulerAngles = neuralController.transform.eulerAngles;
			neuralControllers [i].Reset ();
		}
	}
	void Update () {
		stopAllMutation = stopMutation;
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			int throtalOutputIndex = currentBestGraph.nodeKeyToIndex ["throtal"];

			NuronSource throtalOutput = currentBestGraph.nuronSources [throtalOutputIndex];
			Debug.Log (GameMath.StretchNumber (throtalOutput.currentValue, 0.5f, 5f, 0f, 1f));
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			int torqueOutputIndex = currentBestGraph.nodeKeyToIndex ["torque"];

			NuronSource  torqueOutput	= currentBestGraph.nuronSources[torqueOutputIndex];
			Debug.Log (GameMath.StretchNumber (torqueOutput.currentValue, 0.5f, 5f, 0f, 1f));
		}
	}
	void FixedUpdate () {
		if (Time.fixedTime > timeForFitnessCheck) {
			timeForFitnessCheck = Time.fixedTime + 0.5f;
			bool resetPopulation = true;
			for (int i = 0; i < neuralControllers.Length; i++) {
				float currentFitness = neuralControllers [i].fitness;
				if (currentFitness-0.2f > fitnessList [i]) {
					resetPopulation = false;
					fitnessList [i] = currentFitness;
				}
			}
			if (resetPopulation)
				ResetPopulation ();
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			Debug.Log (neuralControllers[2].throtalOutput.currentValue);
			Debug.Log (neuralControllers[0].torqueOutput.currentValue);

			for (int i = 0; i < currentBestGraph.nuronSources.Length; i++) {
				if (!(currentBestGraph.nuronSources [i] is Nuron))
					continue;
				Nuron neuralNode1 = (Nuron)neuralControllers[0].neuralGraph.nuronSources [i];
				Nuron neuralNode2 = (Nuron)neuralControllers[1].neuralGraph.nuronSources [i];

				for (int j = 0; j < neuralNode1.neuralNode.weights.Length; j++) {
					if (neuralNode1.neuralNode.weights [j] != neuralNode2.neuralNode.weights [j]) {
						Debug.Log (i + ", " + j);
					}
				}
				if (neuralNode1.neuralNode.bias != neuralNode2.neuralNode.bias) {
					Debug.Log (neuralNode1.neuralNode.bias + "," + neuralNode2.neuralNode.bias);
				}
			}
		}
	}
}
