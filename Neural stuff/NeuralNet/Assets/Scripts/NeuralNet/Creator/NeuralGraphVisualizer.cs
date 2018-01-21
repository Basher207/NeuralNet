using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class NuronSource {
	public NeuralGraph sourceGraph;
	public abstract float currentValue {
		get;
	}
	public abstract NuronSource GetCopy ();
}
[System.Serializable]
public class InputNuron : NuronSource {
	public float _currentValue;
	public override float currentValue {
		get {
			return _currentValue;
		}
	}
	public InputNuron () {
		_currentValue = 0f;
	}
	public InputNuron (InputNuron copyOf) {
		_currentValue = copyOf.currentValue;
	}
	public override NuronSource GetCopy () {
		InputNuron newNuron = new InputNuron (this);
		return newNuron;
	}
}
[System.Serializable]
public class Nuron : NuronSource {
	public int [] sourceNuronsIndexes;
	public NeuralNode neuralNode;


	float [] inputs;

	public override float currentValue {
		get {
			for (int i = 0; i < sourceNuronsIndexes.Length; i++) {
				inputs[i] = sourceGraph.nuronSources[sourceNuronsIndexes [i]].currentValue;
			}
			return neuralNode.sigmoidedWeight (inputs);
		}
	}

	public Nuron () {
		
	}
	public Nuron (Nuron copyOf) {
		neuralNode = new NeuralNode (copyOf.neuralNode);
		sourceNuronsIndexes = (int[])copyOf.sourceNuronsIndexes.Clone ();
		inputs = (float[])copyOf.inputs.Clone ();
	}

	public void Intialize () {
		inputs = new float[sourceNuronsIndexes.Length];
		neuralNode = new NeuralNode (sourceNuronsIndexes.Length);

	}
	public override NuronSource GetCopy () {
		Nuron newNuron = new Nuron (this);
		
		return newNuron;
	}
	public void Mutate (float mutationMagnitude) {
		int mutatedIndex = Random.Range (0, neuralNode.weights.Length);

		neuralNode.weights [mutatedIndex] += Random.Range (-mutationMagnitude, mutationMagnitude);
		neuralNode.weights [mutatedIndex] = Mathf.Clamp ((neuralNode.weights [mutatedIndex]), -3, 3);
		//neuralNode.bias += Random.Range (-mutationMagnitude, mutationMagnitude);//continuous 

//		for (int i = 0; i < neuralNode.weights.Length; i++) {
//			neuralNode.weights [i] += Random.Range (-mutationMagnitude, mutationMagnitude);
//			neuralNode.bias += Random.Range (-mutationMagnitude, mutationMagnitude);
//		}
	}
}

public class NeuralGraph {
	public NuronSource [] nuronSources;


	public Dictionary<string, int> inputKeyToIndex;
	public Dictionary<string, int> nodeKeyToIndex;


	public NeuralGraph () {

	}
	public NeuralGraph (NeuralGraph copyOf) {
		nuronSources = new NuronSource[copyOf.nuronSources.Length];

		for (int i = 0; i < nuronSources.Length; i++) {
			NuronSource nuronSource = copyOf.nuronSources [i].GetCopy ();
			nuronSource.sourceGraph = this;
			nuronSources [i] = nuronSource;
		}
		inputKeyToIndex = copyOf.inputKeyToIndex;
		nodeKeyToIndex = copyOf.nodeKeyToIndex;
	}
	public void Mutate (float mutationMagnitude) {
		
//		int indexOfMutation = Random.Range (0, nuronSources.Length);
//		if (nuronSources [indexOfMutation] is Nuron) {
//			Nuron n = (Nuron)nuronSources [indexOfMutation];
//			n.Mutate (mutationMagnitude);
//		}
		for (int i = 0; i < ((nuronSources.Length - inputKeyToIndex.Count) / 2) + 1; i++) {
			int indexOfMutation = Random.Range (0, nuronSources.Length);
			if (nuronSources [indexOfMutation] is Nuron) {
				Nuron n = (Nuron)nuronSources [indexOfMutation];
				n.Mutate (mutationMagnitude);
			}
		}
//		for (int i = 0; i < nuronSources.Length; i++) {
//			if (nuronSources [i] is Nuron) {
//				Nuron n = (Nuron)nuronSources [i];
//				n.Mutate (mutationMagnitude);
//			}
//		}
	}
	public InputNuron GetInputNuron (string key) {
		return (InputNuron)nuronSources[inputKeyToIndex[key]];
	}
}




public class NeuralGraphVisualizer : MonoBehaviour {
	public NeuralGraph neuralGraph;

	public NeuralGraphInput[] neuralGraphInputs;
	public NeuralGraphNode[] neuralGraphOutput;


	void Start () {
		NeuralGraph neuralGraph = GetNeuralGraph ();
	}

	public NeuralGraph GetNeuralGraph () {
		List<GraphSource> graphSources = new List<GraphSource> (GetComponentsInChildren<GraphSource> ());
		NeuralGraphNode [] neuralPoints = GetComponentsInChildren<NeuralGraphNode> ();

		NuronSource [] nuronSources = new NuronSource[graphSources.Count];
		InputNuron [] inputNurons = new InputNuron[neuralGraphInputs.Length];

		NeuralGraph nuralGraph = new NeuralGraph ();

		Dictionary<string, int> inputKeyToIndex = new Dictionary<string, int> ();
		Dictionary<string, int> nodeKeyToIndex = new Dictionary<string, int> ();


		for (int i = 0; i < neuralGraphInputs.Length; i++) {
			int index = graphSources.IndexOf (neuralGraphInputs [i]);
			InputNuron inputNuron = new InputNuron ();
			inputNuron.sourceGraph = nuralGraph;
			nuronSources [index] = inputNuron;

			inputNurons [i] = inputNuron;
			inputKeyToIndex.Add (neuralGraphInputs [i].graphNodeId, index);
		}
		for (int i = 0; i < neuralPoints.Length; i++) {
			int index = graphSources.IndexOf (neuralPoints [i]);
			Nuron nuron = new Nuron ();
			nuron.sourceGraph = nuralGraph;
			nuronSources [index] = nuron;
			nuron.sourceNuronsIndexes = new int[neuralPoints[i].graphSources.Length];
			for (int j = 0; j < nuron.sourceNuronsIndexes.Length; j++) {
				nuron.sourceNuronsIndexes [j] = graphSources.IndexOf (neuralPoints [i].graphSources [j]);
			}

			if (neuralPoints [i].graphNodeId != "") {
				nodeKeyToIndex.Add (neuralPoints [i].graphNodeId, index);
			}
			nuron.Intialize ();
		}


		nuralGraph.nuronSources = nuronSources;
		nuralGraph.inputKeyToIndex = inputKeyToIndex;
		nuralGraph.nodeKeyToIndex = nodeKeyToIndex;


		return nuralGraph;
	}
}
