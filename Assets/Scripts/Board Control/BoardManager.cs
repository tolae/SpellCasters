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

	public GameObject darkness;
	public GameObject space;
	public GameObject hallway;
	public GameObject connected;

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

//		for ( int x = 0; x < columns; x++ ) {
//			for ( int y = 0; y < rows; y++ ) {
//				if ( x == 0 || y == 0 || x == rows - 1 || y == columns - 1 ) {
//					GridSpot tile = new GridSpot( new Vector3( x, y, 0f ), darkness );
//					tile.Changeable = false;
//					map.addTile( tile );
//				}
//				else {
//					GridSpot tile = new GridSpot( new Vector3( x, y, 0f ), darkness );
//					tile.Changeable = true;
//					map.addTile( tile );
//				}
//			}
//		}

		currRoom = 0;
		currFloor = floors[ floor / LEVELS ];
		maxRoom = roomCount.getRandom();

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
			int ranY = Random.Range( 0, rows );
			int ranX = Random.Range( 0, columns );
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
				try {
					if ( !( map.getTile( x1, y1 ).Changeable ) ) //If the tile isn't changeable
						return false;
				} catch ( IndexOutOfRangeException ) {
					//Do nothing, this is expected. Means the gridspot is empty
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
						if ( room.isEdge( x1, y1 ) ) {
							map.addTile( x1, y1, new WallSpot() );
						} else if ( SPACE != 0 &&
								( ( y1 >= y-SPACE && y1 < y ) 
									|| ( y1 < y + height + SPACE && y1 >= y + height )
									|| ( x1 >= x-SPACE && x1 < x )
									|| ( x1 < x + width + SPACE && x1 >= x + width ) ) ) {
							//Creates a filler tile around the room, giving rooms some space. Might remove.
							map.addTile( x1, y1, new DarknessSpot( new Vector3( x1, y1, 0f ), darkness, false ) );
						}

						map.addTile( x1, y1, new RoomSpot() );

					}
				}
			}
		}
	}

	void connectRooms() {
		foreach( Room start in rooms ) {
			foreach( Room end in rooms ) {
				if ( start != end && end.hasOpenDoor() && start.hasOpenDoor() ) {
					GridSpot startDoor = start.getDoorway();
					GridSpot endDoor = end.getDoorway();
					PathGen.Pathfinder lastTurn = PathGen.Pathfinder.None;
					PathGen.Pathfinder noBack = PathGen.Pathfinder.None;

					List< GridSpot > path = new List< GridSpot >();
					path.Add( startDoor );
					map.getTile( endDoor ).type = GridSpot.Type.Doorway;

					path = PathGen.shortestPath( startDoor, endDoor, lastTurn, noBack, path );

					map.getTile( startDoor ).type = GridSpot.Type.Connected;
				}
			}
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

	Room[] getRooms( int[] roomIndex, List< Room > aval ) {
		Room start = aval[ roomIndex[ 0 ] ];
		Room end = aval[ roomIndex[ 1 ] ];

		if ( !start.hasOpenDoor() )
			start = aval[ ( roomIndex[ 1 ] + 1 ) % aval.Count ];
		if ( !end.hasOpenDoor() )
			end = aval[ ( roomIndex[ 0 ] + 1 ) % aval.Count ];

		return new Room[] { start, end };
	}

	int[] getRandomRoom( List< Room > aval ) {
		int start = Random.Range( 0 , aval.Count );
		int end = Random.Range( 0, aval.Count );

		if ( start == end )
			end = ( start + 1 ) % aval.Count;

		return new int[] { start, end };
	}

	void populateMap() {
		for ( int x = 0; x < columns; x++ ) {
			for (int y = 0; y < rows; y++ ) {
				GridSpot spot = map.getTile( x, y );

				if ( spot.type == GridSpot.Type.None || spot.type == GridSpot.Type.Border )
					Instantiate( darkness, spot.coord, Quaternion.identity );
				if ( spot.type == GridSpot.Type.Space )
					Instantiate( space, spot.coord, Quaternion.identity );
				if ( spot.type == GridSpot.Type.Hallway )
					Instantiate( hallway, spot.coord, Quaternion.identity );
				if ( spot.type == GridSpot.Type.Connected )
					Instantiate( connected, spot.coord, Quaternion.identity );
				if ( spot.type == GridSpot.Type.Room )
					Instantiate( currFloor.floorTiles, spot.coord, Quaternion.identity );
				if ( spot.type == GridSpot.Type.Wall )
					Instantiate( currFloor.wallTiles, spot.coord, Quaternion.identity );
			}
		}
	}
		
	public class PathGen {

		public enum Pathfinder {
			Up,
			Down,
			Left,
			Right,
			None,
			Door,
		}
			
		public static List< GridSpot > shortestPath( GridSpot currDoor, GridSpot endDoor, 
			Pathfinder lastTurn, Pathfinder noBack, List< GridSpot> path ) {

			if ( checkForDoor( currDoor ) == Pathfinder.Door && path.Count != 1 ) {
				map.getTile( endDoor ).type = GridSpot.Type.Connected;
				return path;
			}

			List< Pathfinder > shortest = new List< Pathfinder >();
			double upDist = calcUp( currDoor, endDoor );
			double downDist = calcDown( currDoor, endDoor );
			double rightDist = calcRight( currDoor, endDoor );
			double leftDist = calcLeft( currDoor, endDoor );
			double[] distances = new double[] { upDist, downDist, leftDist, rightDist };

			for ( int i = 0; i < 4; i++ ) {
				if ( upDist == minElement( distances ) && distances[ 0 ] != double.MaxValue ) {
					shortest.Add( Pathfinder.Up );
					distances[ 0 ] = double.MaxValue;
				}
				if ( downDist == minElement( distances ) && distances[ 1 ] != double.MaxValue ) {
					shortest.Add( Pathfinder.Down );
					distances[ 1 ] = double.MaxValue;
				}
				if ( leftDist == minElement( distances ) && distances[ 2 ] != double.MaxValue ) {
					shortest.Add( Pathfinder.Left );
					distances[ 2 ] = double.MaxValue;
				}
				if ( rightDist == minElement( distances ) && distances[ 3 ] != double.MaxValue ) {
					shortest.Add( Pathfinder.Right );
					distances[ 3 ] = double.MaxValue;
				}
			}

			if ( checkDirection( shortest[ 0 ], currDoor ) == Pathfinder.None ) {
				Pathfinder dir = shortest[ 0 ];

				if ( lastTurn != dir )
					noBack = lastTurn;

				if ( noBack != dir ) {
					if ( dir == Pathfinder.Up && lastTurn != Pathfinder.Down ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveUp( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Down && lastTurn != Pathfinder.Up ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveDown( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Left && lastTurn != Pathfinder.Right ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveLeft( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Right && lastTurn != Pathfinder.Left ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveRight( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
				}
			} else if ( checkDirection( shortest[ 1 ], currDoor ) == Pathfinder.None ) {
				Pathfinder dir = shortest[ 1 ];

				if ( lastTurn != dir )
					noBack = lastTurn;

				if ( noBack != dir ) {
					if ( dir == Pathfinder.Up && lastTurn != Pathfinder.Down ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveUp( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Down && lastTurn != Pathfinder.Up ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveDown( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Left && lastTurn != Pathfinder.Right ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveLeft( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Right && lastTurn != Pathfinder.Left ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveRight( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
				}
			} else if ( checkDirection( shortest[ 2 ], currDoor ) == Pathfinder.None ) {
				Pathfinder dir = shortest[ 2 ];

				if ( lastTurn != dir )
					noBack = lastTurn;

				if ( noBack != dir ) {
					if ( dir == Pathfinder.Up && lastTurn != Pathfinder.Down ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveUp( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Down && lastTurn != Pathfinder.Up ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveDown( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Left && lastTurn != Pathfinder.Right ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveLeft( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Right && lastTurn != Pathfinder.Left ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveRight( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
				}
			} else if ( checkDirection( shortest[ 3 ], currDoor ) == Pathfinder.None ) {
				Pathfinder dir = shortest[ 3 ];

				if ( lastTurn != dir )
					noBack = lastTurn;

				if ( noBack != dir ) {
					if ( dir == Pathfinder.Up && lastTurn != Pathfinder.Down ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveUp( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Down && lastTurn != Pathfinder.Up ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveDown( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Left && lastTurn != Pathfinder.Right ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveLeft( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
					if ( dir == Pathfinder.Right && lastTurn != Pathfinder.Left ) {
						lastTurn = dir;

						GridSpot nextCoord = map.moveRight( currDoor );

						path.Add( nextCoord );

						return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );
					}
				}
			}

			return continueCurrent( currDoor, endDoor, lastTurn, noBack, path );
		}

		public static List< GridSpot > continueCurrent( GridSpot currDoor, GridSpot endDoor, 
			Pathfinder lastTurn, Pathfinder noBack, List< GridSpot> path ) {

			if ( lastTurn == Pathfinder.Up ) {

				GridSpot nextCoord = map.moveUp( currDoor );

				path.Add( nextCoord );

				return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );

			} 
			else if ( lastTurn == Pathfinder.Down ) {

				GridSpot nextCoord = map.moveDown( currDoor );

				path.Add( nextCoord );

				return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );

			} 
			else if ( lastTurn == Pathfinder.Left ) {

				GridSpot nextCoord = map.moveLeft( currDoor );

				path.Add( nextCoord );

				return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );

			} 
			else if ( lastTurn == Pathfinder.Right ) {

				GridSpot nextCoord = map.moveRight( currDoor );

				path.Add( nextCoord );

				return shortestPath( nextCoord, endDoor, lastTurn, noBack, path );

			}

			Debug.Log( "Why: " + currDoor.coord + "\nLastTurn: " + lastTurn + "\nnoBack: " + noBack );
			return path;
		}


		public static Pathfinder checkForDoor( GridSpot curr ) {
			Pathfinder door = Pathfinder.None;

			if ( checkUp( curr ) == Pathfinder.Door )
				door = Pathfinder.Door;
			else if ( checkDown( curr ) == Pathfinder.Door )
				door = Pathfinder.Door;
			else if ( checkLeft( curr ) == Pathfinder.Door )
				door = Pathfinder.Door;
			else if ( checkRight( curr ) == Pathfinder.Door )
				door = Pathfinder.Door;

			return door;

		}

		public static Pathfinder checkDirection( Pathfinder dir, GridSpot curr ) {
			switch( dir ) {
			case Pathfinder.Up:
				return checkUp( curr );
			case Pathfinder.Down:
				return checkDown( curr );
			case Pathfinder.Left:
				return checkLeft( curr );
			case Pathfinder.Right:
				return checkRight( curr );
			default:
				Debug.Log( "???" );
				return Pathfinder.Door;
			}
		}

		public static double minElement( double[] array ) {
			double smallest = double.MaxValue;
			foreach ( double e in array ) {
				if ( e < smallest )
					smallest = e;
			}

			return smallest;
		}

		public static double calcUp( GridSpot spot, GridSpot end ) {
			float upX = spot.coord.x;
			float upY = spot.coord.y + 1;
			float endX = end.coord.x;
			float endY = end.coord.y;

			return Math.Sqrt( ( ( upX - endX )*( upX - endX ) ) + ( ( upY - endY )*( upY - endY ) ) );
		}

		public static double calcDown( GridSpot spot, GridSpot end ) {
			float upX = spot.coord.x;
			float upY = spot.coord.y - 1;
			float endX = end.coord.x;
			float endY = end.coord.y;

			return Math.Sqrt( ( ( upX - endX )*( upX - endX ) ) + ( ( upY - endY )*( upY - endY ) ) );
		}

		public static double calcRight( GridSpot spot, GridSpot end ) {
			float upX = spot.coord.x + 1;
			float upY = spot.coord.y;
			float endX = end.coord.x;
			float endY = end.coord.y;

			return Math.Sqrt( ( ( upX - endX )*( upX - endX ) ) + ( ( upY - endY )*( upY - endY ) ) );
		}

		public static double calcLeft( GridSpot spot, GridSpot end ) {
			float upX = spot.coord.x - 1;
			float upY = spot.coord.y;
			float endX = end.coord.x;
			float endY = end.coord.y;

			return Math.Sqrt( ( ( upX - endX )*( upX - endX ) ) + ( ( upY - endY )*( upY - endY ) ) );
		}

		//Returns None if the spot ahead is clear
		public static Pathfinder checkUp( GridSpot spot ) {
			int x = (int) spot.coord.x;
			int y = (int) spot.coord.y + 1;

			if ( y == columns )
				return Pathfinder.Up;

			GridSpot up = map.getTile( x, y );

			if ( up.type == GridSpot.Type.Room 
				|| up.type == GridSpot.Type.Wall )
				return Pathfinder.Up;
			else if ( up.type == GridSpot.Type.Doorway )
				return Pathfinder.Door;
			else
				return Pathfinder.None;
		}

		public static Pathfinder checkDown( GridSpot spot ) {
			int x = (int) spot.coord.x;
			int y = (int) spot.coord.y - 1;

			if ( y < 0 )
				return Pathfinder.Down;

			GridSpot down = map.getTile( x, y );

			if ( down.type == GridSpot.Type.Room
				|| down.type == GridSpot.Type.Wall )
				return Pathfinder.Down;
			else if ( down.type == GridSpot.Type.Doorway )
				return Pathfinder.Door;
			else
				return Pathfinder.None;
		}

		public static Pathfinder checkLeft( GridSpot spot ) {
			int x = (int) spot.coord.x - 1;
			int y = (int) spot.coord.y;

			if ( x < 0 )
				return Pathfinder.Left;

			GridSpot left = map.getTile( x, y );

			if ( left.type == GridSpot.Type.Room
				|| left.type == GridSpot.Type.Wall )
				return Pathfinder.Left;
			else if ( left.type == GridSpot.Type.Doorway )
				return Pathfinder.Door;
			else
				return Pathfinder.None;
		}

		public static Pathfinder checkRight( GridSpot spot ) {
			int x = (int) spot.coord.x + 1;
			int y = (int) spot.coord.y;

			if ( x == rows )
				return Pathfinder.Right;

			GridSpot right = map.getTile( x, y );

			if ( right.type == GridSpot.Type.Room
				|| right.type == GridSpot.Type.Wall )
				return Pathfinder.Right;
			else if ( right.type == GridSpot.Type.Doorway )
				return Pathfinder.Door;
			else
				return Pathfinder.None;
		}
	}
}