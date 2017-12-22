using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MovingUnit {

	protected bool hasDirection = false;
	protected List< GridSpot > path = new List< GridSpot >();

	protected override void Start () {
		base.Start ();

		Health = 35;
		Defense = 0;
		Strength = 10;
		Mana = 0;
		ViewRange = 3;
	}

	void Update() {
		if ( path.Count <= 0 ) { hasDirection = false; }
	}

	public void enemyTurn() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerTeam"); //Tag all players and there allies have

		foreach ( GameObject go in enemies ) {
			//Move unit towards him, otherwise keep going in some random direction or not moving
			if ( Detect< Unit >( this.transform.position, go.transform.position ) ) { //Insight
				//preAttemptMove( go.transform.position );
			}
		}

		if ( !hasDirection ) { //hasDirection == false
			Debug.Log( "No path! Generating one..." );

			//preAttemptMove();

			hasDirection = true;
		} else if ( hasDirection ) { //hasDirection == true
			//preAttemptMove();
		}
	}
	/*
	void preAttemptMove( Vector3 enemy ) {
		int moveX = this.transform.position.x - enemy.x;
		int moveY = this.transform.position.y - enemy.y;
		if ( Mathf.Abs( moveX ) > 1 && Mathf.Abs( moveY ) > 1 && Mathf.Abs( moveX ) == Mathf.Abs( moveY ) ) {
			if ( moveX > 0 )
				attemptMove< Player >( 1, 0 );
			else if ( moveX < 0 )
				attemptMove< Player >( -1, 0 );
		}

		attemptMove< Player >( moveX, moveY );
	}*/

	protected override bool attemptMove< T > (int xDir, int yDir) {
		if ( xDir == 0 && yDir == 0 ) {
			
		}
		bool canMove = base.attemptMove< T > (xDir, yDir);

		return canMove;
	}

	protected override void onCantMove<T> (T component) {
		if ( component is Player ) {
			Attack( component );
		}
		GameManager.instance.finishedStack.Add( this.gameObject );
	}

	protected void Attack<T>( T notAlly ) where T : Unit {
		notAlly.Hurt( Strength );
	}

	protected override void onStop() {
		
	}

	protected override bool unitDetect< T >( T component) {
		return true;
	}

	override
	public void Hurt ( int damage ) {
		int loss = damage - Defense;
		if ( loss < 0 )
			loss = 0;
		Health -= loss;
		Debug.Log( Health );
	}

	int Strength{ get; set; }
}