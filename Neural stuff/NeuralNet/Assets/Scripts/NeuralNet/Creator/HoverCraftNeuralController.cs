using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraftNeuralController : MonoBehaviour {


	public float rayCastAngle = 35f;
	public float maxRayCastRange = 1f;
	public NeuralGraph neuralGraph;

	[HideInInspector] public InputNuron sensorLeft;
	[HideInInspector] public InputNuron sensorForward;
	[HideInInspector] public InputNuron sensorRight;

	public NuronSource throtalOutput;
	public NuronSource torqueOutput;

	public bool gizmos;


	[HideInInspector] public Rigidbody2D rigidBod;
	[HideInInspector] public HoverCraft craftController;
	[HideInInspector] public Vector2 boxCollSize;
	[HideInInspector] public BoxCollider2D boxColl;
	[HideInInspector] public Vector2 previousLoc;
	[HideInInspector] public float timeOfStart;

	public float lifeTime {
		get {
			return Time.time - timeOfStart;
		}
	}
	public Vector2 lastFitnessPivotPoint;
	public float fitnessAtPivotReached;
	public float fitness = 0f;
	public const float minPivotRadius = 8f;
//	public const float maxFitness = 1f;

	public Ray2D leftSensorRay {
		get {
			Vector2 origin = boxCollSize / 2f;
			origin.x = -origin.x;

			origin = transform.TransformPoint (origin);

			Vector2 direction = new Vector2 (0f, 1f).AClockWiseRotate (rayCastAngle) ;
			direction = transform.TransformDirection (direction);
			return new Ray2D (origin, direction);
		}
	}
	public Ray2D rightSensorRay {
		get {
			Vector2 origin = boxCollSize / 2f;

			origin = transform.TransformPoint (origin);

			Vector2 direction = new Vector2 (0f, 1f).AClockWiseRotate (-rayCastAngle);
			direction = transform.TransformDirection (direction);
			return new Ray2D (origin, direction);
		}
	}
	public Ray2D forwardSensorRay {
		get {
			Vector2 origin = boxCollSize / 2f;
			origin.x = 0f;

			origin = transform.TransformPoint (origin);

			Vector2 direction = new Vector2 (0f, 1f);
			direction = transform.TransformDirection (direction);

			return new Ray2D (origin, direction);
		}
	}




	public void Awake () {
		rigidBod = GetComponent<Rigidbody2D> ();
		craftController = GetComponent<HoverCraft> ();
		boxColl = GetComponent<BoxCollider2D> ();
		boxCollSize = boxColl.size;
	}
	public void SetNeuralGraph (NeuralGraph sourceGraph, float mutationMag) {
//		if (Mathf.Abs (mutationMag) > 0.0001f) {
			neuralGraph = new NeuralGraph (sourceGraph);

		if (!HovercraftEvolutionManager.stopAllMutation)
			neuralGraph.Mutate (mutationMag);
//		} else {
//			neuralGraph = new NeuralGraph (sourceGraph);
//		}

		timeOfStart = Time.time;
//		} else {
//			Debug.Log ("default graph");
//			neuralGraph = sourceGraph;
//		}

		int throtalOutputIndex 	= neuralGraph.nodeKeyToIndex ["throtal"];
		int torqueOutputIndex 	= neuralGraph.nodeKeyToIndex ["torque"];

		sensorLeft 		= neuralGraph.GetInputNuron ("left");
		sensorForward 	= neuralGraph.GetInputNuron ("front");
		sensorRight 	= neuralGraph.GetInputNuron ("right");

		throtalOutput	= neuralGraph.nuronSources[throtalOutputIndex];
		torqueOutput	= neuralGraph.nuronSources[torqueOutputIndex];

		float neuralVal = torqueOutput.currentValue;
		float sourceVal = sourceGraph.nuronSources [torqueOutputIndex].currentValue;

//		if (neuralVal != sourceVal) {
//			Debug.Log ("neuralVal: " + neuralVal + ", sourceVal: " + sourceVal);
//		}
	}
	void SetGraph () {

	}

	Vector2 startPos;
	public void Reset () {
		fitness = 0f;
		rigidBod.bodyType = RigidbodyType2D.Dynamic;
		rigidBod.velocity = Vector2.zero;
		rigidBod.angularVelocity = 0f;
		previousLoc = transform.position;
		startPos = transform.position;
		this.enabled = true;
		lastFitnessPivotPoint = new Vector2 (55555f, 55555f);
		fitnessAtPivotReached = fitness;
//		swapRays = !swapRays;
	}
	bool swapRays = false;
	public void FixedUpdate () {
		if (lifeTime > 120f) {
			this.enabled = false;
		}
		Vector2 pos = transform.position;

//		float distanceToLastPivot = Vector2.Distance (lastFitnessPivotPoint, pos);
//		if (distanceToLastPivot > minPivotRadius) {
//			lastFitnessPivotPoint = pos;
//			fitnessAtPivotReached = fitness;
//		}
		fitness += Vector2.Distance (transform.position, previousLoc);
		previousLoc = transform.position;
//
//		fitness = Mathf.Min (fitness, fitnessAtPivotReached + minPivotRadius);
//		fitness = Vector2.Distance (startPos, pos);

		Ray2D forwardRay = forwardSensorRay;
		Ray2D rightRay = rightSensorRay;
		Ray2D leftRay = leftSensorRay;

		RaycastHit2D forwardHit = Physics2D.Raycast (forwardRay.origin, forwardRay.direction, maxRayCastRange, LayerMasks.wallMask);
		RaycastHit2D rightHit = Physics2D.Raycast (rightRay.origin, rightRay.direction, maxRayCastRange, LayerMasks.wallMask);
		RaycastHit2D leftHit = Physics2D.Raycast (leftRay.origin, leftRay.direction, maxRayCastRange, LayerMasks.wallMask);

		float forwardDistance = forwardHit ? forwardHit.distance : maxRayCastRange;
		float rightDistance = rightHit ? rightHit.distance : maxRayCastRange;
		float leftDistance = leftHit ? leftHit.distance : maxRayCastRange;

//		if (swapRays) {
//			float temp = rightDistance;
//			rightDistance = leftDistance;
//			leftDistance = temp;
//		}
		sensorRight._currentValue = rightDistance / maxRayCastRange;
		sensorLeft._currentValue = leftDistance / maxRayCastRange;
		sensorForward._currentValue = forwardDistance / maxRayCastRange;
//
		//		Debug.Log ("stuff");
//		Debug.Log ("ForwardSensor: " + sensorForward._currentValue);
//		Debug.Log ("RightSensor: " + sensorRight._currentValue);
//		Debug.Log ("LeftSensor: " + sensorLeft._currentValue);
//		Debug.Log (forwardDistance);
//		Debug.Log (throtalOutput.currentValue);
//		Debug.Log (GameMath.sigmoid (throtalOutput.currentValue));


		craftController.throtal = Mathf.Max(0.2f, throtalOutput.currentValue);//GameMath.StretchNumber (throtalOutput.currentValue, 0.5f, 2f, 0f, 1f);
		craftController.torque = torqueOutput.currentValue; //> 0.5f ? 1f : 0f;//torqueOutput.currentValue;//GameMath.StretchNumber (torqueOutput.currentValue, 0.5f, 2f, 0f, 1f);

//		if (swapRays) {
//			craftController.torque = 1 - craftController.torque;
//		}
	}
	void OnCollisionEnter2D (Collision2D collision) {
		this.enabled = false;
	}

	public void OnDrawGizmosSelected () {
		if (gizmos) {
			Ray2D forwardRay = forwardSensorRay;
			Ray2D rightRay = rightSensorRay;
			Ray2D leftRay = leftSensorRay;

			Gizmos.DrawLine (forwardRay.origin, forwardRay.origin + forwardRay.direction * maxRayCastRange);
			Gizmos.DrawLine (rightRay.origin, rightRay.origin + rightRay.direction * maxRayCastRange);
			Gizmos.DrawLine (leftRay.origin, leftRay.origin + leftRay.direction * maxRayCastRange);
		}
	}
}
