using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {

	private GridSpot[,] map;
	private int rows;
	private int cols;

	public Board( int rows, int cols ) {
		this.rows = rows;
		this.cols = cols;
		map = new GridSpot[ rows, cols ];
	}

	public GridSpot getTile( int x, int y ) {
		return map[ x, y ];
	}

	public GridSpot getTile( GridSpot coord ) {
		return map[ (int) coord.Coord().x, (int) coord.Coord().y ];
	}

	public GridSpot moveUp( GridSpot coord ) {
		map[ (int) coord.Coord().x, 
			(int) coord.Coord().y + 1 ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.Coord().x, 
			coord.Coord().y + 1, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveDown( GridSpot coord ) {
		map[ (int) coord.Coord().x, 
			(int) coord.Coord().y - 1 ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.Coord().x, 
			coord.Coord().y - 1, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveLeft( GridSpot coord ) {
		map[ (int) coord.Coord().x - 1, 
			(int) coord.Coord().y ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.Coord().x - 1, 
			coord.Coord().y, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveRight( GridSpot coord ) {
		map[ (int) coord.Coord().x + 1, 
			(int) coord.Coord().y ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.Coord().x + 1, 
			coord.Coord().y, 0f ), GridSpot.Type.None );
	}

	public void addTile( int x, int y, GridSpot tile ) {
		map[ x, y ] = tile;
	}

	public void addTile( GridSpot coord ) {
		map[ (int) coord.Coord().x, (int) coord.Coord().y ] = coord;
	}
}
