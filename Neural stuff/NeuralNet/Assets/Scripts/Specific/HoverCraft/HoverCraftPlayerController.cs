using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraftPlayerController : MonoBehaviour {
	
	HoverCraft hoverCraft;

	void Awake () {
		hoverCraft = GetComponent<HoverCraft> ();
	}
	void FixedUpdate () {
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		hoverCraft.throtal = yAxis;
		hoverCraft.torque  = GameMath.Map (-xAxis,-1f, 1f, 0f, 1f);
	}
}
