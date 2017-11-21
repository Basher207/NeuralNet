using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPlotter : MonoBehaviour {

	public List<Vector2> points;
	public Transform line;

	public GameObject pointVisualizerPrefab;
	public NeuralNode neuralNode;

	List<Transform> pointVisualizers;
	[HideInInspector] public Transform pointParent;


	void Awake () {
		pointVisualizers = new List<Transform>();
		pointParent = new GameObject ("PointParent").transform;
		pointParent.transform.SetParent (transform, false);
		pointParent.transform.localPosition = Vector3.zero;
	}

	void Update () {
		Train ();
		UpdatePoints ();
		UpdateLine ();

		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition);
			points.Add (mousePos);
		}
	}
	void UpdatePoints () {
		for (int i = pointVisualizers.Count; i < points.Count; i++) {
			GameObject pointVisualizer = Instantiate<GameObject> (pointVisualizerPrefab, pointParent);
			pointVisualizers.Add (pointVisualizer.transform);
		}
		for (int i = 0; i < pointVisualizers.Count; i++) {
			pointVisualizers [i].gameObject.SetActive (i < points.Count);
		}

		for (int i = 0; i < points.Count; i++) {
			pointVisualizers [i].SetLocalXY (points [i]);
		}
	}
	void UpdateLine () {
		Vector2 point1 = new Vector2 (0f, 0f);
		Vector2 point2 = new Vector2 (1f, 0f);

		point1.y = neuralNode.n (point1.x);
		point2.y = neuralNode.n (point2.x);

		line.localPosition = point1;
		line.LocalPointRightAtDirection (point2 - point1);
	}
	void Train () {
		float [][] inputs = new float[points.Count][];
		float [] expectedAnswers = new float[points.Count];

		for (int i = 0; i < points.Count; i++) {
			expectedAnswers [i] = points[i].y;

			inputs[i] = new float[1];
			inputs [i][0] = points [i].x;
		}
		neuralNode.Train (inputs, expectedAnswers);

	}

}
