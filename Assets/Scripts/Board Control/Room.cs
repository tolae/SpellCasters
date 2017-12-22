using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Room {

	private int xLoci, yLoci;
	private int width, height;
	private int connections;

	private List< GridSpot > doorway = new List< GridSpot >();

	private enum Side {
		NORTH,
		SOUTH,
		EAST,
		WEST,
	}

	public Room(int xCoord, int yCoord, int width, int height, int connections) {
		doorway.Clear();

		this.xLoci = xCoord;
		this.yLoci = yCoord;
		this.width = width;
		this.height = height;
		this.connections = connections;
	}

	public void initialize() {
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

	public List< GridSpot > createRoom() {
		List< GridSpot > toReturn = new List< GridSpot >();

		for ( int x = xLoci; x < xLoci + width; x++ ) {
			for ( int y = yLoci; y < yLoci + height; y++ ) {
				if ( isEdge( x, y ) ) {
					toReturn.Add( new GridSpot( new Vector3( x, y, 0f ), GridSpot.SpotType.WALL ) );
				} else {
					toReturn.Add( new GridSpot( new Vector3( x, y, 0f ), GridSpot.SpotType.ROOM ) );
				}
			}
		}

		return toReturn;
	}

	void createDoorway( Side side ) {
		int x = 0; 
		int y = 0;
		if ( side == Side.NORTH ) {
			x = Random.Range( 1, width - 1 );
			y = height - 1;
		} else if ( side == Side.SOUTH ) {
			x = Random.Range( 1, width - 1 );
			y = 0;
		} else if ( side == Side.EAST ) {
			x = width - 1;
			y = Random.Range( 1, height - 1 );
		} else if ( side == Side.WEST ) {
			x = 0;
			y = Random.Range( 1, height - 1 );
		}

		doorway.Add( new GridSpot( new Vector3( x + xLoci, y + yLoci, 0f ), GridSpot.SpotType.DOOR_OPEN ) );
	}

	bool isEdge( GridSpot spot ) {
		int x = (int) spot.Coord().x;
		int y = (int) spot.Coord().y;
		return isEdge(x , y);
	}

	bool isEdge( int x, int y ) {
		if ( x == xLoci && y == yLoci )
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

	public List< GridSpot > getDoorways() {
		return doorway;
	}

	public void editDoorways(int index, GridSpot door) {
		doorway.Insert(index, door);
	}

	override
	public string ToString() {
		return "ROOM DIM " + width + "x" + height;
	}
}
