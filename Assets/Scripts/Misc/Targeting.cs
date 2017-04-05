using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {
	
	public GameObject movingTile;
	public GameObject stillTile;
	protected List< GameObject > tiles = new List< GameObject >();
	protected int range = 5;
	protected int AoE = 1;

	protected virtual void Start() {
		Debug.Log( "Spell started" );
	}

	public virtual void Cast( Vector3 start, Projectile.Face face ) {
		GameManager.instance.TargetControl = true;
		createMovingTarget( AoE );
		createStillTarget( range );
	}

	protected virtual void ActualCast( Vector3 start ) {
		Debug.Log("Casted");
		foreach ( GameObject t in tiles )
			Destroy( t );
		GameManager.instance.TargetControl = false;
	}

	protected virtual void Update() {
		if ( !GameManager.instance.TargetControl ) { return; }

		if ( Input.GetKeyDown( KeyCode.W ) )
			AttemptMove( new Vector3( 0f, 1f, 0f ) );
		else if ( Input.GetKeyDown( KeyCode.S ) )
			AttemptMove( new Vector3( 0f, -1f, 0f ) );
		else if ( Input.GetKeyDown( KeyCode.A ) )
			AttemptMove( new Vector3( -1f, 0f, 0f ) );
		else if ( Input.GetKeyDown( KeyCode.D ) )
			AttemptMove( new Vector3( 1f, 0f, 0f ) );
		else if ( Input.GetKeyDown( KeyCode.Return ) )
			ActualCast( transform.position );
	}

	void AttemptMove( Vector3 move ) {
		float xT = transform.position.x + move.x;
		float yT = transform.position.y + move.y;
		float xC = GameManager.instance.character.transform.position.x;
		float yC = GameManager.instance.character.transform.position.y;

		if ( Mathf.Abs( xT - xC ) + Mathf.Abs( yT - yC ) > range - 1 ) {
			Debug.Log( "Unable to move! Outside range! " );
			return;
		} else {
			transform.position += move;
		}
	}

	void createStillTarget( int range ) {

		if ( range == 1 ) {
			createSOne( stillTile );
		} else {
			createSTile( range, stillTile );
		}

	}

	void createMovingTarget( int AoE ) {

		if ( AoE == 1 ) {
			createMOne( movingTile );
		} else {
			createMTile( AoE, stillTile );
		}

	}

	void createMOne( GameObject t ) {
		t = Instantiate( t, transform.position, 
			Quaternion.identity, transform) as GameObject;

		tiles.Add( t );
	}

	void createMTile( int AoE, GameObject t ) {

		float x = AoE - 1;
		float y1 = 0;
		float y2 = 0;

		for ( int iterations = AoE * 2; iterations > 1; iterations-- ) {
			if ( x == range - 1 || x == -1 * range + 1 ) {
				Vector3 pos = new Vector3( x, 0f, 0f );

				GameObject tile = Instantiate( t, transform.position + pos, 
					Quaternion.identity, transform ) as GameObject;

				tiles.Add( tile );
			} else {
				Vector3 pos1 = new Vector3( x, y1, 0f );
				Vector3 pos2 = new Vector3( x, y2, 0f );

				GameObject t1 = Instantiate( t, transform.position + pos1, 
					Quaternion.identity, transform ) as GameObject;

				GameObject t2 = Instantiate( t, transform.position + pos2, 
					Quaternion.identity, transform ) as GameObject;

				tiles.Add( t1 );
				tiles.Add( t2 );
			}
				
			if ( x > 0 ) {
				y1--;
				y2++;
			} else {
				y1++;
				y2--;
			}
			x--;
		}

		createMovingTarget( AoE - 1 );
	}

	void createSOne( GameObject t ) {
		t = Instantiate( t, transform.position, 
			Quaternion.identity ) as GameObject;

		tiles.Add( t );
	}

	void createSTile( int range, GameObject t ) {
		
		float x = range - 1;
		float y1 = 0;
		float y2 = 0;

		for ( int iterations = range * 2; iterations > 1; iterations-- ) {
			if ( x == range - 1 || x == -1 * range + 1 ) {
				Vector3 pos = new Vector3( x, 0f, 0f );

				GameObject tile = Instantiate( t, transform.position + pos, 
					Quaternion.identity ) as GameObject;

				tiles.Add( tile );
			} else {
				Vector3 pos1 = new Vector3( x, y1, 0f );
				Vector3 pos2 = new Vector3( x, y2, 0f );

				GameObject t1 = Instantiate( t, transform.position + pos1, 
					Quaternion.identity ) as GameObject;

				GameObject t2 = Instantiate( t, transform.position + pos2, 
					Quaternion.identity ) as GameObject;

				tiles.Add( t1 );
				tiles.Add( t2 );
			}

			if ( x > 0 ) {
				y1--;
				y2++;
			} else {
				y1++;
				y2--;
			}
			x--;
		}

		createStillTarget( range - 1 );
	}

}
