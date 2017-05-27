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

	public void enemyTurn() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerTeam"); //Tag all players and there allies have

		foreach ( GameObject go in enemies ) {
			//Move unit towards him, otherwise keep going in some random direction or not moving
			if ( distFrom( go ) <= ViewRange ) { //Insight
				path.Clear();

				path = PathGen.getPath( GameManager.instance.boardScript.getBoard(), 
				GameManager.instance.boardScript.getUnitSpot( this.gameObject ), 
				GameManager.instance.boardScript.getUnitSpot( go ), 
				path, EnumManager.PathStyle.Unit );

				hasDirection = true;
			}
		}

		if ( !hasDirection ) {
			path.Clear();

			path = PathGen.getPath( GameManager.instance.boardScript.getBoard(),
			GameManager.instance.boardScript.getUnitSpot( this.gameObject ),
			GameManager.instance.boardScript.getRandomTile(),
			path, EnumManager.PathStyle.Unit );

			hasDirection = true;
		} else if ( hasDirection ) {
			attemptMove< Player >( (int) ( path[ 0 ].Coord().x - this.transform.position.x ),
			(int) ( path[ 0 ].Coord().y - this.transform.position.y ) );
		}

		if ( path.Count <= 0 ) { hasDirection = false; }
	}

	protected override bool attemptMove< T > (int xDir, int yDir) {
		bool canMove = base.attemptMove< T > (xDir, yDir);

		return canMove;
	}

	protected override void onCantMove<T> (T component) {
		if ( component is Player ) {
			Attack( component );
		}
	}

	protected void Attack<T>( T notAlly ) where T : Unit {
		notAlly.Hurt( Strength );
	}

	override
	protected void onStop() {
		GameManager.instance.finishedStack.Add( this.gameObject );
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