using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNode {
	public float [] weights;
	public float bias;
	public float improvementPower = -0.01f;


	public NeuralNode (int inputNodes) {
		weights = new float[inputNodes];
		for (int i = 0; i < weights.Length; i++) {
			weights [i] = 0f;
		}
//		RandomValues ();
	}
	public NeuralNode (NeuralNode neuralNode) {
		weights = (float[])neuralNode.weights.Clone ();
		bias = neuralNode.bias;
		improvementPower = neuralNode.improvementPower;
	}
	public void RandomValues () {
		for (int i = 0; i < weights.Length; i++) {
			weights [i] = Random.Range (-1f, 1f);
		}
//		bias = Random.Range (-1f, 1f);
	}
	public float sigmoidedWeight (params float [] inputs) {
		if (inputs.Length != weights.Length) {
			Debug.LogError ("Inputs.Length != weights.Length !!!!!");
			return 0.5f;
		}
		float z = 0f;
		for (int i = 0; i < weights.Length; i++) {
			z += weights [i] * inputs [i];
		}
		z += bias;

		return GameMath.sigmoid (z*20);
	}
	public float n (params float [] inputs) {
		if (inputs.Length != weights.Length) {
			Debug.LogError ("Inputs.Length != weights.Length !!!!!");
			return 0.5f;
		}
		float z = 0f;
		for (int i = 0; i < weights.Length; i++) {
			z += weights [i] * inputs [i];
		}
		z += bias;

		return z;
	}
	public void Train (float [] inputs, float expectedAnswer) {
		float z = n (inputs);

		float error = (z - expectedAnswer);
//		float squaredError = error * error;



		float [] weightsDerivatives = new float[weights.Length];
		float biasDerivative = 0f;

		for (int i = 0; i < weightsDerivatives.Length; i++) {
			weightsDerivatives[i] = 2 * (error) * inputs[i];
		}
		biasDerivative = 2 * (error);

		for (int i = 0; i < weights.Length; i++) {
			weights [i] += weightsDerivatives [i] * improvementPower;
		}
		bias += biasDerivative * improvementPower;

//		return 0f;
	}
	public void Train (float [][] inputs, float [] expectedAnswers) {

//		float [] totalWeightsDerivatives = new float[weights.Length];
//		float totalBiasDerivatives = 0f;

		for (int j = 0; j < inputs.Length; j++) {
			Train (inputs [j], expectedAnswers [j]);
		}

//		return 0f;
	}

}
