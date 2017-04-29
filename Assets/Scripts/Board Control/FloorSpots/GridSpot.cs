using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridSpot : MonoBehaviour
{
	protected Vector3 coord;
	protected GameObject 

	public GridSpot ( Vector3 coord ) {
		this.coord = coord;
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}