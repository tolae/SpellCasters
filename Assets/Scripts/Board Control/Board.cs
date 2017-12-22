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

	public Board( Board toCopy ) {
		this.rows = toCopy.rows;
		this.cols = toCopy.cols;
		this.map = toCopy.map.Clone() as GridSpot[,];
	}

	public GridSpot getTile( int x, int y ) {
		return map[ x, y ];
	}

	public GridSpot getTile( GridSpot spot ) {
		return map[ (int) spot.Coord().x, (int) spot.Coord().y ];
	}

	public GridSpot getRandomTile< E >( E type ) {
		return null;
	}

	public void addTile( int x, int y, GridSpot tile ) {
		map[ x, y ] = tile;
	}

	public void addTile( GridSpot spot ) {
		map[ (int) spot.Coord().x, (int) spot.Coord().y ] = spot;
	}

	public void addTileList< E >( List< E > list ) where E: GridSpot {
		foreach( GridSpot spot in list ) {
			addTile(spot);
		}
	}
}
