using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour {

	[Range (1, 100)]
	public int fixedUpdatesPerSecond = 50;
	[Range (0, 100)]
	public float timeScale = 1f;

	void Update () {
		Time.fixedDeltaTime = 1f / fixedUpdatesPerSecond;
		Time.timeScale = timeScale;
	}
}
