using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMath {


	public static float sigmoid (float x) {
		return 1/(1 + Mathf.Exp(-x));
	}
	public static float slope (float b) {
		return 2 * (b - 4);
	}
	public static float Map (float value, float fromMin, float fromMax, float toMin, float toMax) {
		return Mathf.Clamp01((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin;
	}
	public static Vector2 AClockWiseRotate (this Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
	public static float StretchNumber (float num, float pivot, float scale, float min, float max) {
		return Mathf.Clamp (pivot + ((num - pivot) * scale) , min, max);
	}
}