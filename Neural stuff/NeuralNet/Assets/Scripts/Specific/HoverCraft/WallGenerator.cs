using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {

	public float roadWidth = 3f;
//	public Transform trackPoints;
	public GameObject trackSide;

	public class Road {
		public Vector2 [] leftSide;
		public Vector2 [] rightSide;
		public float totalLength;
	}
	public Road GenerateRoad () {
		Road road = new Road ();

		int numberOfCorners = transform.childCount;

		Vector2 [] positions = new Vector2[numberOfCorners];
		Vector2 [] delta 	 = new Vector2[numberOfCorners];

		for (int i = 0; i < numberOfCorners; i++) {
			positions[i] = transform.GetChild (i).position;
		}
		for (int i = 0; i < numberOfCorners; i++) {
			int nextIndex = i + 1;
			if (nextIndex >= numberOfCorners)
				nextIndex = 0;
			delta [i] = (positions [nextIndex] - positions [i]);
			road.totalLength += delta [i].magnitude;
			delta [i].Normalize ();
		}

		road.leftSide  = new Vector2[numberOfCorners];
		road.rightSide = new Vector2[numberOfCorners];

		for (int i = 0; i < numberOfCorners; i++) {
			int previousCornerIndex = i - 1;
			if (previousCornerIndex < 0)
				previousCornerIndex = numberOfCorners - 1;
			
			Vector2 toNextCorner = delta [i];
			Vector2 fromPrevCorner = delta [previousCornerIndex];

			Vector2 cornerDelta = Quaternion.Lerp ((Quaternion.FromToRotation (toNextCorner, fromPrevCorner)), Quaternion.identity, 0.5f) * toNextCorner;

			Vector2 towardsRightCorner = cornerDelta.normalized.AClockWiseRotate (-90) * roadWidth;


			road.rightSide [i] = positions [i] + towardsRightCorner;
			road.leftSide  [i] = positions [i] - towardsRightCorner;
		}

		return road;
	}
	void Start () {
		if (trackSide) {		
			Road road = GenerateRoad ();
			int roadLength = road.leftSide.Length;

			GameObject [] roadL = new GameObject[roadLength];
			GameObject [] roadR = new GameObject[roadLength];
			GameObject roadParent = new GameObject ("RoadParent");
			for (int i = 0; i < roadLength; i++) {
				int nextIndex = i + 1;
				int previousCornerIndex = i - 1;
				if (nextIndex >= roadLength) {
					nextIndex = 0;
				}
				if (previousCornerIndex < 0) {
					previousCornerIndex = roadLength - 1;
				}
				roadL[i] = Instantiate<GameObject> (trackSide, roadParent.transform);
				roadR[i] = Instantiate<GameObject> (trackSide, roadParent.transform);

				Vector2 lPoint = road.leftSide[i], lNextPoint = road.leftSide[nextIndex];
				Vector2 rPoint = road.rightSide[i], rNextPoint = road.rightSide[nextIndex];

				Vector2 lDelta = lNextPoint - lPoint;
				Vector2 rDelta = rNextPoint - rPoint;

				Quaternion lRot = Quaternion.FromToRotation (Vector2.right, lDelta);
				Quaternion rRot = Quaternion.FromToRotation (Vector2.right, rDelta);

				roadL [i].transform.rotation = lRot;
				roadR [i].transform.rotation = rRot;

				roadL [i].transform.position = lPoint + lDelta / 2f;
				roadR [i].transform.position = rPoint + rDelta / 2f;

				roadL [i].transform.localScale = new Vector3 (lDelta.magnitude, 0.3f, 1f);
				roadR [i].transform.localScale = new Vector3 (rDelta.magnitude, 0.3f, 1f);
			}
//			float trackLengthLeft = road.totalLength + 30f;
//
//			while (trackLengthLeft > 0) {
//				
//				trackLengthLeft--;
//			}
		}
	}
	public void OnDrawGizmos () {
		Road road = GenerateRoad ();
		int roadLength = road.leftSide.Length;
		Gizmos.color = Color.green;

		for (int i = 0; i < roadLength; i++) {
			int nextIndex = i + 1;
			if (nextIndex >= roadLength) {
				nextIndex = 0;
			}
			Gizmos.DrawLine (road.leftSide [i], road.leftSide [nextIndex]);
		}
		for (int i = 0; i < roadLength; i++) {
			int nextIndex = i + 1;
			if (nextIndex >= roadLength) {
				nextIndex = 0;
			}
			Gizmos.DrawLine (road.rightSide [i], road.rightSide[nextIndex]);
		}
	}
}
