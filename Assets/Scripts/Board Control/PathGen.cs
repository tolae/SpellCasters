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

	public static List< GridSpot > getPath( Board map, GridSpot start, GridSpot end, List< GridSpot > path ) {
		//Step 1: Initialization
		copyMap = map.copy();

		pGridSpot first = new pGridSpot( start );
		first.mark = 0;

		copyMap.addTile( first );

		path = waveExpansion( start, end, path, 1 );

		return path;
	}

	private static List< GridSpot > waveExpansion( GridSpot curr, GridSpot end, List< GridSpot > path, int marker ) {
		List< pGridSpot > neighbors = getNeighbors( curr );

		foreach ( pGridSpot neighbor in neighbors ) {
			if ( neighbor == null ) //No neighbor, collapse the branch
				return path;
			
			neighbor.mark = marker; //Marks each neighbor with the distance
			if ( neighbor.Coord().Equals( end.Coord() ) ) { //Found endpoint
				path = backtrack( neighbor, path ); //Step 3: Backtrace
				break;
			} else {
				waveExpansion( neighbor, end, path, marker++ );
			}
		}
		copyMap = null; //Step 4: Clearance
		return path; //Step 5: Return
	}

	private static List< GridSpot > backtrack( pGridSpot curr, List< GridSpot > path ) {
		if ( curr.mark <= 0 ) //Found start point
			return path;
		else {
			List< pGridSpot > neighbors = getNeighbors( curr );
			foreach ( pGridSpot back in neighbors ) {
				if ( back.mark < curr.mark ) {
					path.Add( back );
					path = backtrack( back, path );
					break;
				}
			}
			return path;
		}
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

		pGridSpot neighbor = null;

		try {
			neighbor = new pGridSpot( copyMap.getTile( x, y ) );
		} catch ( System.IndexOutOfRangeException e ) {
			Debug.LogError( e );
		}

		return neighbor;
	}
	
}