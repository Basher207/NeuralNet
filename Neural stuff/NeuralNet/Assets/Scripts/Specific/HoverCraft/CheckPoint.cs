using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public float fitnessGain = 2f;
	public CheckPoint previousCheckPoint;
	public CheckPoint nextCheckPoint;

	void OnTriggerEnter2D (Collider2D coll) {
		HoverCraftNeuralController neuralController = coll.GetComponent<HoverCraftNeuralController> ();
		neuralController.fitness += fitnessGain;
	}
}
