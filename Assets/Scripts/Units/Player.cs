using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MovingUnit {

	protected EnumManager.Face face = EnumManager.Face.Right;
	protected EnumManager.Face animFace = EnumManager.Face.Right;
	protected Animator animator;
	protected SpellBook spellbook;

	protected override void Start() {
		base.Start();

		spellbook = GetComponent< SpellBook >();
		animator = GetComponent< Animator >();
	}

	// Update is called once per frame
	protected void Update () {
		if ( !GameManager.instance.playerTurn ) { return; }

		if ( !GameManager.instance.TargetControl ) {

			if ( Input.GetKeyDown( KeyCode.A ) ) { //Left
				if ( attemptMove< Unit >( -1, 0 ) ) {
					moveAnim( EnumManager.Face.Left );
				}
				face = EnumManager.Face.Left;
				animFace = EnumManager.Face.Left;
			}
			else if ( Input.GetKeyDown( KeyCode.D ) ) { //Right
				if ( attemptMove< Unit >( 1, 0 ) ) {
					moveAnim( EnumManager.Face.Right );
				}
				face = EnumManager.Face.Right;
				animFace = EnumManager.Face.Right;
			}
			else if ( Input.GetKeyDown( KeyCode.W ) ) { //Up
				if ( attemptMove< Unit >( 0, 1 ) ) {
					moveAnim( face );
				}
				face = EnumManager.Face.Up;
			}
			else if ( Input.GetKeyDown( KeyCode.S ) ) { //Down
				if ( attemptMove< Unit >( 0, -1 ) ) {
					moveAnim( face );
				}
				face = EnumManager.Face.Down;
			}
			else if ( Input.GetKeyDown( KeyCode.Q ) ) {
				GetComponent< BoxCollider2D >().enabled = false;
				spellbook.castFirst( transform.position, face);
				GetComponent< BoxCollider2D >().enabled = true;
			}
			else if ( Input.GetKeyDown( KeyCode.E ) ) {
				spellbook.nextSpell();
			}
			else if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
				changeAnim( EnumManager.Face.Left );
			}
			else if ( Input.GetKeyDown( KeyCode.RightArrow ) ) {
				changeAnim( EnumManager.Face.Right );
			}
			else if ( Input.GetKeyDown( KeyCode.UpArrow ) ) {
				changeAnim( EnumManager.Face.Up );
			}
			else if ( Input.GetKeyDown( KeyCode.DownArrow ) ) {
				changeAnim( EnumManager.Face.Down );
			}
		}
	}

	protected override bool attemptMove< T > ( int xDir, int yDir ) {
		bool canMove = base.attemptMove< T >( xDir, yDir );
		return canMove;
	}
		
	protected override void onCantMove<T> ( T component ) {
		Debug.Log( component );
		GameManager.instance.playerTurn = true;
	}

	protected override void onStop() {
		animator.SetTrigger( "Stopping" );
		float x = transform.position.x;
		float y = transform.position.y;
		rb2D.MovePosition( new Vector2( Mathf.Round( x ), Mathf.Round( y ) ) );
		GameManager.instance.playerTurn = false;
	}

	void moveAnim( EnumManager.Face nextState ) {
		if ( nextState == face || nextState == EnumManager.Face.Down 
			|| nextState == EnumManager.Face.Up ) {
			animator.SetTrigger( "Moving" );
		} else if ( nextState != animFace ) {
			animator.SetTrigger( "Swap" );
		}
	}

	void changeAnim( EnumManager.Face nextState ) {
		if ( face != nextState ) {
			face = nextState;

			if ( face != EnumManager.Face.Down && face != EnumManager.Face.Up
				&& animFace != nextState ) {
				animator.SetTrigger( "Change" );
				animFace = nextState;
			}
		}
	}
}