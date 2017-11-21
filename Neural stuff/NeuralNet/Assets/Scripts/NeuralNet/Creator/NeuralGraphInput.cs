using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralGraphInput : GraphSource {
	
	public NuronSource nuron;

	public override NuronSource Nuron () {
		return nuron;
	}
}
