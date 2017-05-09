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

	public void addTile( int x, int y, GridSpot tile ) {
		map[ x, y ] = tile;
	}

	public void addTile( GridSpot coord ) {
		map[ (int) coord.Coord().x, (int) coord.Coord().y ] = coord;
	}

	public Board copy() {
		Board toReturn = new Board( this.rows, this.cols );
		toReturn.map = ( GridSpot[,] ) this.map.Clone();
		return toReturn;
	}
}
