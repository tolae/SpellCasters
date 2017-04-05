using System;
using UnityEngine;

public class GridSpot
{
	public enum Type {
		Border,
		Room,
		Wall,
		Hallway,
		Connected,
		Doorway,
		Space,
		None,
	}

	public Vector3 coord;
	public Type type;

	public GridSpot ( Vector3 coord, Type type ) {
		this.coord = coord;
		this.type = type;
	}

	override
	public String ToString() {
		return "X: " + this.coord.x + "\tY: " + this.coord.y;
	}
}