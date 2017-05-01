using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridSpot : MonoBehaviour
{
	protected Vector3 coord; //Coordinates of this gridspot on the map
	protected new GameObject gameObject; //Hides the fact that this gridspot is also a gameobject (GUI)
	protected Unit[] unit; //Units that are currently in this gridspot
	public bool Changeable{ get; private set; } //If the tile can be changed

	public GridSpot ( Vector3 coord, GameObject obj, bool changeable ) {
		this.coord = coord;
		this.gameObject = obj;
		this.Changeable = changeable;
	}

	public Unit getUnit() {
		return unit[0];
	}

	public Vector3 Coord() {
		return coord;
	}
		
	public void Instantiate() {
		Instantiate( this.gameObject, coord, Quaternion.identity );
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}