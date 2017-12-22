using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridSpot
{
	public enum SpotType {
		DARK, DOOR_OPEN, DOOR_CLOSED, HALL, WALL, ROOM, MARKED,
	}
	
	protected Vector3 coord; //Coordinates of this gridspot on the map
	protected Unit[] unit; //Units that are currently in this gridspot
	protected SpotType type;

	public GridSpot ( Vector3 coord, SpotType type ) {
		this.coord = coord;
		this.type = type;
	}

	public Unit getUnit() {
		return unit[0];
	}

	public Vector3 Coord() {
		return coord;
	}

	public SpotType getType() {
		return this.type;
	}

	public void setType(SpotType type) {
		this.type = type;;
	}

	public bool compareType(SpotType type) {
		return type == this.type;
	}

	public bool Changeable() {
		switch (this.type) {
		case SpotType.DARK:
			return true;
		case SpotType.DOOR_OPEN:
			return false;
		case SpotType.DOOR_CLOSED:
			return false;
		case SpotType.HALL:
			return true;
		case SpotType.ROOM:
			return false;
		case SpotType.WALL:
			return true;
		default:
			return true;
		}
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}