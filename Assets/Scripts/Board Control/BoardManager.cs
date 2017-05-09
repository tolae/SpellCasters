using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		
		public int minimum, maximum;
		
		public Count( int min, int max ) {
			this.minimum = min;
			this.maximum = max;
		}
		
		public int getRandom() {
			if ( minimum == maximum )
				return maximum;
			return Random.Range( minimum, maximum + 1 );
		}
		
	}

	public static int columns = 24;
	public static int rows = 24;
	
	public Count roomCount = new Count( 4, 8 );
	public Count roomDim = new Count( 5, 20 );
	public Count roomConn = new Count( 2, 2 );
	public Count itemCount = new Count( 3, 10 );
	public Count mobCount = new Count( 0, 5 );

	public int LEVELS = 5;

	public Floor[] floors;
	private Floor currFloor;

	private GameObject darknessTile;
	private GameObject hallwayTile;
	private GameObject doorTile;
	private GameObject roomTiles;
	private GameObject wallTiles;

	public GameObject debugTile;

	public int SPACE = 1;

	private int maxTries = 10000;
	private int tryCount = 0;
	private int currRoom;
	private int maxRoom;
	private List< Room > rooms = new List< Room >();
	private static Board map;

	public void instantiate( int floor ) {
		rooms.Clear();
		map = new Board( rows, columns );

		currRoom = 0; //Choosing the floor
		currFloor = floors[ ( (int) floor / LEVELS ) ];
		//Instantiating the tiles
		darknessTile = currFloor.darknessTile;
		hallwayTile = currFloor.floorTiles;
		doorTile = currFloor.doorTile;
		wallTiles = currFloor.wallTiles;
		roomTiles = currFloor.roomTiles;

		maxRoom = roomCount.getRandom();

		for ( int x = 0; x < columns; x++ ) {
			for ( int y = 0; y < rows; y++ ) {
				if ( x == 0 || y == 0 || x == rows - 1 || y == columns - 1 ) {
					GridSpot tile = new GridSpot( new Vector3( x, y, 0f ), darknessTile, false );
					map.addTile( tile );
				}
				else {
					GridSpot tile = new GridSpot( new Vector3( x, y, 0f ), darknessTile, true );
					map.addTile( tile );
				}
			}
		}

		generateRoom();
		try {
			connectRooms();
		} catch ( Exception e ) {
			Debug.LogError( e.StackTrace );
		}
		populateMap();
	}

	void generateRoom() {
		if ( currRoom >= maxRoom || tryCount > maxTries ) {
			return;
		} else {
			int ranY = Random.Range( 2, rows - 1 ); //To ensure that the path gen can get to the room
			int ranX = Random.Range( 2, columns - 1 );
			int ranWidth = roomDim.getRandom();
			int ranHeight = roomDim.getRandom();
			int ranConn = roomConn.getRandom();
			if ( createRoom( ranX, ranY, ranWidth, ranHeight ) ) {
				Room room = new Room();
				room.initialize( ranX, ranY, ranWidth, ranHeight, ranConn, currFloor );
				finalizeRoom( ranX, ranY, ranWidth, ranHeight, room );
				rooms.Add( room );
				currRoom++;
			}
			tryCount++;
			generateRoom();
		}
	}

	bool createRoom( int x, int y, int width, int height ) {
		for ( int x1 = x; x1 < x + width; x1++ ) {
			for ( int y1 = y; y1 < y + height; y1++ ) {
				if ( !map.getTile( x1, y1 ).Changeable ) { //If the tile isn't changeable
					return false;
				}
			}
		}
		return true;
	}

	void finalizeRoom( int x, int y, int width, int height, Room room ) {
		for ( int x1 = x-SPACE; x1 < x + width + SPACE; x1++ ) {
			if ( x1 >= 0 ) {
				for ( int y1 = y-SPACE; y1 < y + height + SPACE; y1++ ) {
					if ( y1 >= 0 && map.getTile( x1, y1 ).Changeable ) {

						map.addTile( new RoomSpot( new Vector3( x1, y1, 0f), roomTiles, false, room ) );

						if ( room.isEdge( x1, y1 ) ) { //Creates the walls around the room
							map.addTile( new WallSpot( new Vector3( x1, y1, 0f), wallTiles, false ) );
						}

						if ( SPACE != 0 &&
								( ( y1 >= y-SPACE && y1 < y ) 
									|| ( y1 < y + height + SPACE && y1 >= y + height )
									|| ( x1 >= x-SPACE && x1 < x )
									|| ( x1 < x + width + SPACE && x1 >= x + width ) ) ) {
							//Creates a filler tile around the room, giving rooms some space. Might remove.
							map.addTile( x1, y1, new DarknessSpot( new Vector3( x1, y1, 0f ), debugTile, false ) );
						}
					}
				}
			}
		}
	}

	void connectRooms() {
		Room roomOne = rooms[ 0 ];
		Room roomTwo = rooms[ 1 ];

		List< GridSpot > path = new List< GridSpot >();
		path = PathGen.getPath( map, roomOne.getDoorway(), roomTwo.getDoorway(), path );

		foreach ( GridSpot spot in path ) {
			map.addTile( spot );
		}
	}

	public GameObject placePlayer( GameObject character ) {
		GameObject player = new GameObject();
		foreach ( Room spawnRoom in rooms ) {
			if ( spawnRoom.hasDoor() ) {
				int randomX = Random.Range( spawnRoom.getX() + 1, spawnRoom.getX() + spawnRoom.getWidth() - 1 );
				int randomY = Random.Range( spawnRoom.getY() + 1, spawnRoom.getY() + spawnRoom.getHeight() - 1 );

				Vector3 placement = new Vector3( randomX, randomY, 0f );
				player = Instantiate( character, placement, Quaternion.identity ) as GameObject;
				return player;
			}
		}

		return player;
	}

	public void placeEnemies( int enemies ) {

		while ( enemies > 0 ) {
			GameObject randomEnemy = currFloor.enemies[ Random.Range( 0, currFloor.enemies.Length - 1 ) ];
			int roomIndex = Random.Range( 0, rooms.Count - 1 );
			int randomX = Random.Range( rooms[ roomIndex ].getX() + 1, 
				rooms[ roomIndex ].getX() + rooms[ roomIndex ].getWidth() - 1 );
			int randomY = Random.Range( rooms[ roomIndex ].getY() + 1, 
				rooms[ roomIndex ].getY() + rooms[ roomIndex ].getHeight() - 1 );

			Vector3 placement = new Vector3( randomX, randomY );
			Instantiate( randomEnemy, placement, Quaternion.identity );

			enemies--;
		}

	}

	void populateMap() {
		for ( int x = 0; x < columns; x++ ) {
			for (int y = 0; y < rows; y++ ) {
				GridSpot spot = map.getTile( x, y );

				Instantiate( spot.initSpot(), spot.Coord(), Quaternion.identity );
			}
		}
	}
}