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
		roomTiles = currFloor.roomTiles;
		wallTiles = currFloor.wallTiles;

		maxRoom = roomCount.getRandom();

		generateRoom();
		try {
			connectRooms();
		} catch ( Exception e ) {
			Debug.LogError( "Error connecting rooms.\n" + e.StackTrace );
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
				Room room = new Room(ranX, ranY, ranWidth, ranHeight, ranConn);
				room.initialize();
				map.addTileList( room.createRoom() );
				rooms.Add( room );
				currRoom++;
			}
			tryCount++;
			generateRoom();
		}
	}

	bool createRoom( int x, int y, int width, int height ) {
		if (x + width > columns - 1 || y + height > rows - 1)
			return false;

		for (int i = x; i < x + width; i++) {
			if ((map.getTile(i, y) != null && !(map.getTile(i, y).Changeable())) ||
			(map.getTile(i, height) != null && !(map.getTile(i, height).Changeable())))
				return false;
		}

		for (int j = y; j < y + height; j++) {
			if ((map.getTile(x, j) != null && !(map.getTile(x, j).Changeable())) ||
			(map.getTile(width, j) != null && !(map.getTile(width, j).Changeable())))
				return false;
		}

		return true;
	}

	void connectRooms() {
		//Graph the nodes so all rooms are accessible
	}

	void populateMap() {
		GridSpot spot = null;
		for ( int x = 0; x < columns; x++ ) {
			for (int y = 0; y < rows; y++ ) {
				if ((spot = map.getTile( x, y )) == null)
					spot = new GridSpot(new Vector3(x,y), GridSpot.SpotType.DARK);

				switch (spot.getType()) {
					case GridSpot.SpotType.DARK:
						Instantiate(darknessTile, spot.Coord(), Quaternion.identity);
						break;
					case GridSpot.SpotType.HALL:
						Instantiate(hallwayTile, spot.Coord(), Quaternion.identity);
						break;
					case GridSpot.SpotType.DOOR_CLOSED:
						Instantiate(doorTile, spot.Coord(), Quaternion.identity);
						break;
					case GridSpot.SpotType.WALL:
						Instantiate(wallTiles, spot.Coord(), Quaternion.identity);
						break;
					case GridSpot.SpotType.ROOM:
						Instantiate(roomTiles, spot.Coord(), Quaternion.identity);
						break;
					default:
						Debug.LogError("Error instantiating tiles.");
						break;
				}
			}
		}
	}

	public GameObject placePlayer( GameObject character ) {
		GameObject player = new GameObject();
		foreach ( Room spawnRoom in rooms ) {
			int randomX = Random.Range( spawnRoom.getX() + 1, spawnRoom.getX() + spawnRoom.getWidth() - 1 );
			int randomY = Random.Range( spawnRoom.getY() + 1, spawnRoom.getY() + spawnRoom.getHeight() - 1 );

			Vector3 placement = new Vector3( randomX, randomY, 0f );
			player = Instantiate( character, placement, Quaternion.identity ) as GameObject;
			return player;
		}

		return player;
	}

	public void placeEnemies() {

		int enemies = mobCount.getRandom();

		while ( enemies > 0 ) {
			GameObject randomEnemy = currFloor.enemies[ Random.Range( 0, currFloor.enemies.Length - 1 ) ];
			int roomIndex = Random.Range( 0, rooms.Count - 1 );
			int randomX = Random.Range( rooms[ roomIndex ].getX() + 1, 
				rooms[ roomIndex ].getX() + rooms[ roomIndex ].getWidth() - 1 );
			int randomY = Random.Range( rooms[ roomIndex ].getY() + 1, 
				rooms[ roomIndex ].getY() + rooms[ roomIndex ].getHeight() - 1 );

			Vector3 placement = new Vector3( randomX, randomY );
			GameManager.instance.enemyStack.Add( 
			Instantiate( randomEnemy, placement, Quaternion.identity ) as GameObject );

			enemies--;
		}

	}

	public GridSpot getRandomTile() {
		int ranRoom = Random.Range( 0, rooms.Count );
		int ranX = Random.Range( 1, rooms[ ranRoom ].getWidth() - 1 );
		int ranY = Random.Range( 1, rooms[ ranRoom ].getHeight() - 1 );
		return map.getTile( rooms[ ranRoom ].getX() + ranX, rooms[ ranRoom ].getY() + ranY );
	}

	public Board getBoard() {
		return map;
	}
}