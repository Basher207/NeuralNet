using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions  {

	public static void SetXY (this Transform transform, Vector2 position) {
		Vector3 pos = position;
		pos.z = transform.position.z;
		transform.position = pos;
	}
	public static void SetLocalXY (this Transform transform, Vector2 position) {
		Vector3 pos = position;
		pos.z = transform.localPosition.z;
		transform.localPosition = pos;
	}
	public static Vector2 ScaleWith (this Vector2 vec2, Vector2 scaleWith) {
		scaleWith.Scale (vec2);
		return scaleWith;
	}

	public static void LocalPointRightAtDirection (this Transform transform, Vector2 direction) {
		direction.Normalize ();
		transform.localEulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg);
	}
}
