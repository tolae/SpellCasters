﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridSpot
{
	protected Vector3 coord; //Coordinates of this gridspot on the map
	protected GameObject gameObject; //Hides the fact that this gridspot is also a gameobject (GUI)
	protected Unit[] unit; //Units that are currently in this gridspot
	public bool Changeable{ get; protected set; } //If the tile can be changed

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
		
	public GameObject initSpot() {
		return this.gameObject;
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}