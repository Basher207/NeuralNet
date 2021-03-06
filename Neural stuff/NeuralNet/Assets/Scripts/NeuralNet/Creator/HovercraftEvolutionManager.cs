﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftEvolutionManager : MonoBehaviour {
	public NeuralGraph [] bestGraphsSoFar;
	public float [] bestGraphsFitnesses;

	[HideInInspector] public List<float> bestFitnessOnDeath;

	public void ResetController (HoverCraftNeuralController hoverCraft) {
		NeuralGraph sourceGraph = bestGraphsSoFar[0];
		for (int i = 0; i < bestGraphsSoFar.Length; i++) {
			if (Random.value > 0.5f) {
				sourceGraph = bestGraphsSoFar [i];
				break;
			}
		}
		hoverCraft.SetNeuralGraph (sourceGraph, mutaitonMag);
		hoverCraft.transform.position = neuralController.transform.position;
		hoverCraft.transform.eulerAngles = neuralController.transform.eulerAngles;
		hoverCraft.Reset ();
		bestFitnessOnDeath.Add (bestGraphsFitnesses [0]);
	}
	public float mutaitonMag;

//	public float minMutationMag = 0.05f;
//	public float maxMutaitonMag = 1f;

	public int increaseStages = 20;

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
		bestFitnessOnDeath = new List<float> ();


		for (int i = 0; i < population; i++) {
			HoverCraftNeuralController neuralControllerCopy = Instantiate<GameObject> (neuralController.gameObject).GetComponent<HoverCraftNeuralController> ();
			neuralControllerCopy.manager = this;
			neuralControllerCopy.indexOfHoverCraft = i;
			neuralControllerCopy.transform.SetParent (craftsParent);
			neuralControllerCopy.SetNeuralGraph (graph, mutaitonMag);
			neuralControllerCopy.Reset ();
			neuralControllerCopy.gameObject.SetActive (true);
			neuralControllers [i] = neuralControllerCopy;
		}
		bestGraphsSoFar = new NeuralGraph[6];
		bestGraphsFitnesses = new float[6];

		for (int i = 0; i < bestGraphsSoFar.Length; i++) {
			bestGraphsSoFar[i] = neuralControllers [i].neuralGraph;
		}

		currentBestFitness = 0f;
		currentBestGraph = neuralControllers [0].neuralGraph;
	}

//	void ResetPopulation () {
//		HoverCraftNeuralController bestController = neuralControllers [0];
//		bestController.fitness = currentBestFitness;
//
//		for (int i = 1; i < neuralControllers.Length; i++) {
//			if (neuralControllers [i].fitness > bestController.fitness) {
////				Debug.Log ("Beat! " + neuralControllers[i].fitness + ", oldBest: " + bestController.fitness);
//				bestController = neuralControllers [i];
//			}
//		}
//		if (bestController.fitness <= currentBestFitness) {
//			bestController.neuralGraph = currentBestGraph;
//			bestController.fitness = currentBestFitness;
//
////			float mutaitonDelta = (maxMutaitonMag - minMutationMag) / increaseStages;
////			mutaitonMag = Mathf.Clamp (mutaitonMag + mutaitonDelta, minMutationMag, maxMutaitonMag); 
//		} else {
//			currentBestGraph = bestController.neuralGraph;
//			currentBestFitness = bestController.fitness;
//
////			mutaitonMag = minMutationMag;
//		}
//
//		fitnessList [0] = 0f;
//		neuralControllers [0].SetNeuralGraph (bestController.neuralGraph, 0f);
//		neuralControllers [0].neuralGraph = graph;
//		neuralControllers [0].transform.position = neuralController.transform.position;
//		neuralControllers [0].transform.eulerAngles = neuralController.transform.eulerAngles;
//		neuralControllers [0].Reset ();
//
//		for (int i = 1; i < neuralControllers.Length; i++) {
//			fitnessList [i] = 0f;
//			neuralControllers [i].SetNeuralGraph (bestController.neuralGraph, mutaitonMag);
//			neuralControllers [i].transform.position = neuralController.transform.position;
//			neuralControllers [i].transform.eulerAngles = neuralController.transform.eulerAngles;
//			neuralControllers [i].Reset ();
//		}
//	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			string outCSV = "";
			for (int i = 0; i < bestFitnessOnDeath.Count; i++) {
				outCSV += bestFitnessOnDeath[i].ToString ();
				outCSV += "\n";
			}
			Debug.Log (outCSV, this);
		}
	}
	void FixedUpdate () {
		if (Time.fixedTime > timeForFitnessCheck) {
			timeForFitnessCheck = Time.fixedTime + 0.5f;
			for (int i = 0; i < neuralControllers.Length; i++) {
				float currentFitness = neuralControllers [i].fitness;
				if (currentFitness-0.2f > fitnessList [i]) {
					fitnessList [i] = currentFitness;

					int bestSpot = -1;

					for (int j = bestGraphsFitnesses.Length - 1; j >= 0; j--) {
						if (currentFitness > bestGraphsFitnesses [j]) {
							bestSpot = j;
						}
					}
					if (bestSpot != -1) {
						bestGraphsFitnesses [bestSpot] = currentFitness;
						bestGraphsSoFar[bestSpot] = neuralControllers[i].neuralGraph;
					}
				}
			}
		}
	}
}
