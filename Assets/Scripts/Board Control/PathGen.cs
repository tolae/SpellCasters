using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGen {

	private class pGridSpot: GridSpot {

		public int mark{ get; set; }
		
		public pGridSpot( GridSpot spot ) : base( spot.Coord(), spot.initSpot(), spot.Changeable ) {
			mark = -1; //-1 means its unmarked
		}
	}

	private static Board copyMap;
	private static List< pGridSpot > stack = new List< pGridSpot >();

	public static List< GridSpot > getPath( Board map, GridSpot start, GridSpot end, List< GridSpot > path ) {
		//Step 1: Initialization
		copyMap = new Board( map );

		pGridSpot first = new pGridSpot( start );
		first.mark = 0;

		copyMap.addTile( first );

		path = waveExpansion( start, end, path, 1 ); //Step 2: Wave expand

		pGridSpot goBack = new pGridSpot( end );  //Step 3: Backtrace
		goBack.mark = int.MaxValue;
		path = backtrack( goBack, path );

		//Step 5: Clearance
		copyMap = null;

		return path;
	}

	private static List< GridSpot > waveExpansion( GridSpot curr, GridSpot end, List< GridSpot > path, int marker ) {
		if ( curr.Coord().Equals( end.Coord() ) ) { //Found endpoint
			path.Add( curr ); //End point
			return path;
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
				waveExpansion( next, end, path, marker ); //Go one more time
				return path;
			} else {
				waveExpansion( next, end, path, marker );
			}
		}

		return path;
	}

	private static List< GridSpot > backtrack( pGridSpot curr, List< GridSpot > path ) {
		if ( curr.mark <= 0 ) { //Found start point
			path.Add( curr );
			return path;
		} else {
			List< pGridSpot > neighbors = getBackTrackNeighbors( curr );
			foreach ( pGridSpot back in neighbors ) {
				if ( back == null ) {  }
				else {
					path.Add( back );
					path = backtrack( back, path );
					return path;
				}
			}
			return path;
		}
	}

	private static List< pGridSpot > getBackTrackNeighbors( pGridSpot curr ) {
		List< pGridSpot > toReturn = new List< pGridSpot >();

		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 0f, 1f, 0f ) ) ); //Up neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 0f, -1f, 0f ) ) ); //Down neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( 1f, 0f, 0f ) ) ); //Right neighbor
		toReturn.Add( getBackTrackNeigh( curr, new Vector3( -1f, 0f, 0f ) ) ); //Left neighbor

		return toReturn;
	}

	private static pGridSpot getBackTrackNeigh( pGridSpot curr, Vector3 toLook ) {
		int x = (int) ( curr.Coord().x + toLook.x );
		int y = (int) ( curr.Coord().y + toLook.y );

		if ( x < 0 || x >= BoardManager.rows ||
			y < 0 || y >= BoardManager.columns ) {
			return null;
		}

		if ( copyMap.getTile( x, y ).GetType() == typeof( pGridSpot ) ) {
			if ( curr.mark > ( (pGridSpot) copyMap.getTile( x, y ) ).mark ) {
				return ( (pGridSpot) copyMap.getTile( x, y ) ); //If the next spot mark is less than the current marker
			}
		}

		return null;
	}

	private static List< pGridSpot > getNeighbors( GridSpot curr ) {
		List< pGridSpot > toReturn = new List< pGridSpot >();

		toReturn.Add( getNeigh( curr, new Vector3( 0f, 1f, 0f ) ) ); //Up neighbor
		toReturn.Add( getNeigh( curr, new Vector3( 0f, -1f, 0f ) ) ); //Down neighbor
		toReturn.Add( getNeigh( curr, new Vector3( 1f, 0f, 0f ) ) ); //Right neighbor
		toReturn.Add( getNeigh( curr, new Vector3( -1f, 0f, 0f ) ) ); //Left neighbor

		return toReturn;
	}

	private static pGridSpot getNeigh( GridSpot curr, Vector3 toLook ) {
		int x = (int) ( curr.Coord().x + toLook.x );
		int y = (int) ( curr.Coord().y + toLook.y );

		if ( x < 0 || x >= BoardManager.rows ||
			y < 0 || y >= BoardManager.columns ) {
			return null;
		}

		System.Type tile = copyMap.getTile( x, y ).GetType();

		if ( !( tile == typeof( pGridSpot ) ) && !( tile == typeof( RoomSpot ) ) && !( tile == typeof( WallSpot ) ) ) {
			return new pGridSpot( copyMap.getTile( x, y ) ); //If this point hasn't been visited
		} else {
			return null; //If this point has been visited
		}
	}
	
}