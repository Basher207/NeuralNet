using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMasks : MonoBehaviour {

	public static int layerMaskIndexOfHoverCraft = 8;
	public static int layerMaskIndexOfWall = 9;

	public static int hoverCraftMask = 1 << layerMaskIndexOfHoverCraft;
	public static int wallMask 	   = 1 << layerMaskIndexOfWall;
}
