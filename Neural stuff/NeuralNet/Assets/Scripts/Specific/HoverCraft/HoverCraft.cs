using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraft : MonoBehaviour {

	public float maxThrotalForce = 5f;
	public float maxTorqueForce = 5f;

	public float throtal;
	public float torque;

	[HideInInspector] public Rigidbody2D rigidBod;

	void Start () {
		rigidBod = GetComponent<Rigidbody2D> ();
	}
	void FixedUpdate () {
		if (rigidBod.bodyType != RigidbodyType2D.Static) {
			rigidBod.velocity = (transform.up * maxThrotalForce);//transform.up * throtal * maxThrotalForce);
			rigidBod.angularVelocity = (GameMath.Map (torque, 0f, 1f, -1f, 1f) * maxTorqueForce);
		}
	}
	void OnCollisionEnter2D (Collision2D collision) {
		rigidBod.bodyType = RigidbodyType2D.Static;
	}
}
