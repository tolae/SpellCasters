/**
 * This class is designed to generate the most optimal path from one location to another based on the given grid. This
 * is used for connecting rooms by doorways and generating the path for enemy AI to take to get to target location.
 * 
 * Creator: Ethan Tola
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGen {

	/**
	 * Supporting class that is used only in PathGen. Used to help mark the distance from the start of the wave
	 * expansion. Extends Gridspot to avoid conflicts with the original board's general gridspots.
	 * 
	 * Creator: Ethan Tola
	 */
	private class pGridSpot: GridSpot {
		//Distance from start location of the wave expansion
		public int mark{ get; set; }
		
		public pGridSpot( GridSpot spot ) : base( spot.Coord(), spot.getType() ) {
			this.mark = -1; //-1 means its unmarked
			this.type = GridSpot.SpotType.MARKED;
		}
	}
	//Temp map as to not mess with the actual map
	private static Board copyMap;
	//Helps organize the order of which to mark each tile. Used for bug fixing. Do not delete.
	private static List< pGridSpot > stack = new List< pGridSpot >();

	/**
	 * Returns a list of gridspots, each with a coord to denote the path that is most optimal between the start and end
	 * coordinates. Left as generic so it can be used in anyway.
	 *
	 * param map: Current state of the board.
	 * param start: Start location for which the path deviates from.
	 * param end: End location for which the path to go to.
	 * param: path: The path from start to end.
	 * return: The most optimal path between start and end.
	 */
	public static List< GridSpot > getPath( Board map, GridSpot start, GridSpot end, List< GridSpot > path, 
	EnumManager.PathStyle style ) {
		//Step 1: Initialization
		copyMap = new Board( map );

		pGridSpot first = new pGridSpot( start );
		first.mark = 0;

		copyMap.addTile( first );

		if ( style == EnumManager.PathStyle.Board ) //Step 2: Wave expand
			boardWaveExpansion( start, end, 1 );
		if ( style == EnumManager.PathStyle.Unit )
			unitWaveExpansion( start, end, 1 );

		path.Add( end );
		pGridSpot goBack = new pGridSpot( end );  //Step 3: Backtrace
		goBack.mark = int.MaxValue;
		path = backtrack( goBack, path );

		//Step 5: Clearance
		copyMap = null;

		return path;
	}
	/**
	 * This is step 2 of Lee's Wave Expansion algorithm. It marks each spot in a radial direction from the current 
	 * position with the current marker.
	 *
	 * param curr: Current position of the wave expansion. Where to expan out from.
	 * param end: Goal location.
	 * param marker: Current distance from the start location.
	 */
	private static void boardWaveExpansion( GridSpot curr, GridSpot end, int marker ) {
		if ( curr.Coord().Equals( end.Coord() ) ) { //Found endpoint
			return;
		} 

		List< pGridSpot > neighbors = getNeighbors( curr );

		foreach ( pGridSpot neighbor in neighbors ) {
			if ( neighbor == null ) {  } //No neighbor, collapse the branch
			else {
				neighbor.mark = marker;
				
				copyMap.addTile( neighbor ); //Add marked tile to the temp map

				stack.Add( neighbor ); //Add marked tile to the bottom of the stack
			}
		}

		while ( stack.Count > 0 ) {
			marker += 1;
			pGridSpot next = stack[ 0 ];
			stack.Remove( next );
			if ( next.Coord().Equals( end.Coord() ) ) {
				boardWaveExpansion( next, end, marker ); //Go one more time
				return;
			} else {
				boardWaveExpansion( next, end, marker );
			}
		}

		return;
	}
	//Same as above expect is uses a the helper method getUnitNeighbors() which has different conditions.
	private static void unitWaveExpansion( GridSpot curr, GridSpot end, int marker ) {
		if ( curr.Coord().Equals( end.Coord() ) ) { //Found endpoint
			return;
		} 

		List< pGridSpot > neighbors = getUnitNeighbors( curr );

		foreach ( pGridSpot neighbor in neighbors ) {
			if ( neighbor == null ) {  } //No neighbor, collapse the branch
			else {
				neighbor.mark = marker;
				
				copyMap.addTile( neighbor ); //Add marked tile to the temp map

				stack.Add( neighbor ); //Add marked tile to the bottom of the stack
			}
		}

		while ( stack.Count > 0 ) {
			marker += 1;
			pGridSpot next = stack[ 0 ];
			stack.Remove( next );
			if ( next.Coord().Equals( end.Coord() ) ) {
				unitWaveExpansion( next, end, marker ); //Go one more time
				return;
			} else {
				unitWaveExpansion( next, end, marker );
			}
		}

		return;
	}
	/**
	 * This is step 3 of Lee's Wave Expansion Algorithm. Starting from the end location, it works backwards towards the
	 * start location, each time going to the first coord with a lower marker than the previous. It does this until it 
	 * reachs the start location (when the mark is 0).
	 *
	 * param curr: Current location to backtrack from.
	 * param path: The most optimal path from the start to the end point.
	 * return: The most optimal path from the start to the end point.
	 */
	private static List< GridSpot > backtrack( pGridSpot curr, List< GridSpot > path ) {
		if ( curr.mark <= 0 ) { //Found start point
			curr.setType(GridSpot.SpotType.HALL);
			path.Add( curr );
			return path;
		} else {
			List< pGridSpot > neighbors = getBackTrackNeighbors( curr );
			foreach ( pGridSpot back in neighbors ) {
				if ( back == null ) {  }
				else {
					back.setType(GridSpot.SpotType.HALL);
					path.Add( back );
					path = backtrack( back, path );
					return path;
				}
			}
			return path;
		}
	}
//==============================================Helper Methods========================================================//
/** NOTICE: ALL Neighbors HELPER METHODS HAVE THE FOLLOWING COMMENTS
 * Helper method for PathGen. Used to get the *four* neighbors of the current spot. These neighbors are currently: up, down, left
 * and right only.
 *
 * param curr: The current position to deviate from.
 * return: A list of neighbors from the current position.
 */
/** NOTICE: ALL Neigh HELPER METHODS HAVE THE FOLLOW COMMENTS
 * Helper method for PathGen. Used to get a single neighbor based on the current position and the place to look.
 *
 * param curr: Current position on the map.
 * param toLook: Position to look to from the current position.
 * return: The gridspot from the current position described by the toLook param.
 */
 //NOTICE: ALL FOLLOWING HELPER METHODS HAVE COMMENTS DENOTING WHO THEY'RE INTENDED FOR
 	/*
 	* Use for ground units only
 	*/
	private static List< pGridSpot > getUnitNeighbors( GridSpot curr ) {
		List< pGridSpot > toReturn = new List< pGridSpot >();

		toReturn.Add( getUnitNeigh( curr, new Vector3( 0f, 1f, 0f ) ) ); //Up neighbor
		toReturn.Add( getUnitNeigh( curr, new Vector3( 0f, -1f, 0f ) ) ); //Down neighbor
		toReturn.Add( getUnitNeigh( curr, new Vector3( 1f, 0f, 0f ) ) ); //Right neighbor
		toReturn.Add( getUnitNeigh( curr, new Vector3( -1f, 0f, 0f ) ) ); //Left neighbor

		return toReturn;
	}
	/*
 	* Use for ground units only
 	*/
	private static pGridSpot getUnitNeigh( GridSpot curr, Vector3 toLook ) {
		int x = (int) ( curr.Coord().x + toLook.x );
		int y = (int) ( curr.Coord().y + toLook.y );

		if ( x < 0 || x >= BoardManager.rows ||
			y < 0 || y >= BoardManager.columns ) {
			return null;
		}

		GridSpot tile = copyMap.getTile( x, y );

		if ( tile.compareType(GridSpot.SpotType.HALL) || 
		tile.compareType(GridSpot.SpotType.DOOR_OPEN) ||
		tile.compareType(GridSpot.SpotType.DOOR_CLOSED) ||
		tile.compareType(GridSpot.SpotType.ROOM) ) {
			return new pGridSpot( copyMap.getTile( x, y ) ); //If this point hasn't been visited
		} else {
			return null; //If this point has been visited
		}
	}
	//Use for backtrack method only
	private static List< pGridSpot > getBackTrackNeighbors( pGridSpot curr ) {
		List< pGridSpot > toReturn = new List< pGridSpot >();

		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 0f, 1f, 0f ) ) ); //Up neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 0f, -1f, 0f ) ) ); //Down neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 1f, 0f, 0f ) ) ); //Right neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( -1f, 0f, 0f ) ) ); //Left neighbor

		return toReturn;
	}
	//Use for backtrack method only
	private static pGridSpot getBackTrackNeigh( pGridSpot curr, Vector3 toLook ) {
		int x = (int) ( curr.Coord().x + toLook.x );
		int y = (int) ( curr.Coord().y + toLook.y );

		if ( x < 0 || x >= BoardManager.rows ||
			y < 0 || y >= BoardManager.columns ) {
			return null;
		}

		if ( (copyMap.getTile( x, y ) != null ) && copyMap.getTile(x, y).compareType(GridSpot.SpotType.MARKED)) {
			if ( curr.mark > ( (pGridSpot) copyMap.getTile( x, y ) ).mark ) {
				return ( (pGridSpot) copyMap.getTile( x, y ) ); //If the next spot mark is less than the current marker
			}
		}

		return null;
	}
	//Use for board wave expansion only
	private static List< pGridSpot > getNeighbors( GridSpot curr ) {
		List< pGridSpot > toReturn = new List< pGridSpot >();

		toReturn.Add( getNeigh( curr, new Vector3( 0f, 1f, 0f ) ) ); //Up neighbor
		toReturn.Add( getNeigh( curr, new Vector3( 0f, -1f, 0f ) ) ); //Down neighbor
		toReturn.Add( getNeigh( curr, new Vector3( 1f, 0f, 0f ) ) ); //Right neighbor
		toReturn.Add( getNeigh( curr, new Vector3( -1f, 0f, 0f ) ) ); //Left neighbor

		return toReturn;
	}
	//Use for board wave expansion only
	private static pGridSpot getNeigh( GridSpot curr, Vector3 toLook ) {
		int x = (int) ( curr.Coord().x + toLook.x );
		int y = (int) ( curr.Coord().y + toLook.y );

		if ( x < 0 || x >= BoardManager.rows ||
			y < 0 || y >= BoardManager.columns ) {
			return null;
		}

		GridSpot tile = copyMap.getTile( x, y );

		if (tile == null)
			return new pGridSpot( new GridSpot(new Vector3(x, y), GridSpot.SpotType.MARKED) );

		if (!(tile.compareType(GridSpot.SpotType.MARKED) ) && 
		!(tile.compareType(GridSpot.SpotType.ROOM) ) && 
		!(tile.compareType(GridSpot.SpotType.WALL) ) ) {
			return new pGridSpot( copyMap.getTile( x, y ) ); //If this point hasn't been visited
		} else {
			return null; //If this point has been visited
		}
	}
	
}