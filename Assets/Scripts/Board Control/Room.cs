using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Room {

	private int xLoci, yLoci;
	private int width, height;
	private bool north;
	private bool south;
	private bool east;
	private bool west;

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

		this.north = true;
		this.south = true;
		this.east = true;
		this.west = true;

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
			}
		} while ( doorway.Count != connections );
	}

	void createDoorway( Side side ) {
		int x = 0; 
		int y = 0;
		if ( side == Side.NORTH && north) {
			x = Random.Range( 1, width - 1 );
			y = height - 1;
			north = false;
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.SOUTH && south) {
			x = Random.Range( 1, width - 1 );
			y = 0;
			south = false;
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.EAST && east) {
			x = width - 1;
			y = Random.Range( 1, height - 1 );
			east = false;
			doorway.Add( new DoorwaySpot( new Vector3( x + xLoci, y + yLoci, 0f ), doorTile, false ) );
		} else if ( side == Side.WEST && west) {
			x = 0;
			y = Random.Range( 1, height - 1 );
			west = false;
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

	public GridSpot getDoorway() {
		GridSpot toReturn = doorway[ this.openDoor ];
		this.openDoor++;
		return toReturn;
	}

	public bool hasOpenDoor() {
		return openDoor < doorway.Count;
	}

	public bool hasDoor() {
		return openDoor > 0;
	}

	override
	public string ToString() {
		return "ROOM DIM " + width + "x" + height;
	}
}
