using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphSource : MonoBehaviour {
	public abstract NuronSource Nuron ();
	public string graphNodeId;
}
public class NeuralGraphNode : GraphSource {

	public GraphSource [] graphSources;
	public Nuron nuron;


	public override NuronSource Nuron () {
		return nuron;
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.white;
		if (graphSources != null) {
			for (int i = 0; i < graphSources.Length; i++) {
				GraphSource source = graphSources [i];
				if (source) {
					Vector3 tPos = source.transform.position;
					Gizmos.DrawLine (transform.position, tPos);
				}
			}
		}
	}
}
