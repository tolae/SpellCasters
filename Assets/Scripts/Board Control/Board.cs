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
		return map[ (int) coord.coord.x, (int) coord.coord.y ];
	}

	public GridSpot moveUp( GridSpot coord ) {
		map[ (int) coord.coord.x, 
			(int) coord.coord.y + 1 ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.coord.x, 
			coord.coord.y + 1, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveDown( GridSpot coord ) {
		map[ (int) coord.coord.x, 
			(int) coord.coord.y - 1 ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.coord.x, 
			coord.coord.y - 1, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveLeft( GridSpot coord ) {
		map[ (int) coord.coord.x - 1, 
			(int) coord.coord.y ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.coord.x - 1, 
			coord.coord.y, 0f ), GridSpot.Type.None );
	}

	public GridSpot moveRight( GridSpot coord ) {
		map[ (int) coord.coord.x + 1, 
			(int) coord.coord.y ].type = GridSpot.Type.Hallway;

		return new GridSpot( new Vector3( coord.coord.x + 1, 
			coord.coord.y, 0f ), GridSpot.Type.None );
	}

	public void addTile( int x, int y, GridSpot tile ) {
		map[ x, y ] = tile;
	}

	public void addTile( GridSpot coord ) {
		map[ (int) coord.coord.x, (int) coord.coord.y ] = coord;
	}
}
