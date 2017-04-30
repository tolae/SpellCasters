using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridSpot : MonoBehaviour
{
	protected Vector3 coord; //Coordinates of this gridspot on the map
	protected new GameObject gameObject; //Hides the fact that this gridspot is also a gameobject (GUI)
	protected Unit unit; //Unit that is currently in this gridspot
	public bool Changeable{ get; set; } //If the tile can be changed
	public bool Unit{ get; set; } //If there is a unit on this gridspot

	public GridSpot ( Vector3 coord, GameObject obj ) {
		this.coord = coord;
		this.gameObject = obj;
	}

	public GridSpot ( Vector3 coord, GameObject obj, bool changeable ) {
		this.coord = coord;
		this.gameObject = obj;
		this.Changeable = changeable;
	}

	public Vector3 Coord() {
		return coord;
	}
		
	public void Instantiate() {
		Instantiate( this.gameObject, coord, Quaternion.identity );
	}

	public Unit getUnit() {
		if ( Unit )
			return unit;
		else {
			Debug.LogWarning( "Trying to grab unit from a tile " +
				"that has no unit or cannot hold units!" );
			return null;
		}
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}