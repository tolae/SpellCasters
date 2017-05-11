﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Room {

	private int xLoci, yLoci;
	private int width, height;

	private int openDoor;

	private List< DoorwaySpot > doorway = new List< DoorwaySpot >();

	private GameObject doorTile;
	private GameObject floorTiles;
	private GameObject wallTiles;

	private enum Side {
		NORTH,
		SOUTH,
		EAST,
		WEST,
	}

	public void initialize( int xCoord, int yCoord, int width, int height, int connections, Floor floor ) {
		doorway.Clear();

		doorTile = floor.doorTile;
		floorTiles = floor.floorTiles;
		wallTiles = floor.wallTiles;

		openDoor = 0;

		this.xLoci = xCoord;
		this.yLoci = yCoord;
		this.width = width;
		this.height = height;

		do {
			switch ( Random.Range( 0, 4 ) ) {
				case 0:
				createDoorway( Side.NORTH );
				break;
				case 1:
				createDoorway( Side.SOUTH );
				break;
				case 2:
				createDoorway( Side.WEST );
				break;
				case 3:
				createDoorway( Side.EAST );
				break;
				default:
				createDoorway( Side.SOUTH );
				break;
			}
		} while ( doorway.Count != connections );
	}

	void createDoorway( Side side ) {
		int x = 0; 
		int y = 0;
		if ( side == Side.NORTH ) {
			x = Random.Range( 1, width - 1 );
			y = height - 1;
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.SOUTH ) {
			x = Random.Range( 1, width - 1 );
			y = 0;
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.EAST ) {
			x = width - 1;
			y = Random.Range( 1, height - 1 );
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.WEST ) {
			x = 0;
			y = Random.Range( 1, height - 1 );
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		}
	}

	public bool isEdge( GridSpot spot ) {
		int x = (int) spot.Coord().x;
		int y = (int) spot.Coord().y;
		if ( x ==  xLoci && y == yLoci )
			return true;
		else if ( x == xLoci && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci + width - 1 && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci + width - 1 && y == yLoci )
			return true;
		else if ( x >= xLoci && y == yLoci )
			return true;
		else if ( x >= xLoci && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci && y >= yLoci )
			return true;
		else if ( x == xLoci + width - 1 && y >= yLoci )
			return true;

		return false;
	}

	public bool isEdge( int x, int y ) {
		if ( x ==  xLoci && y == yLoci )
			return true;
		else if ( x == xLoci && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci + width - 1 && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci + width - 1 && y == yLoci )
			return true;
		else if ( x >= xLoci && y == yLoci )
			return true;
		else if ( x >= xLoci && y == yLoci + height - 1 )
			return true;
		else if ( x == xLoci && y >= yLoci )
			return true;
		else if ( x == xLoci + width - 1 && y >= yLoci )
			return true;

		return false;
	}

	public int getX() {
		return xLoci;
	}

	public int getY() {
		return yLoci;
	}

	public int getWidth() {
		return width;
	}

	public int getHeight() {
		return height;
	}

	public DoorwaySpot getDoorway() {
		return doorway[ this.openDoor ];
	}

	public List< DoorwaySpot > getDoorways() {
		return doorway;
	}

	override
	public string ToString() {
		return "ROOM DIM " + width + "x" + height;
	}
}
