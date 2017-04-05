using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MovingUnit, Unit {

	protected Projectile.Face face = Projectile.Face.RIGHT;
	protected Projectile.Face animFace = Projectile.Face.RIGHT;
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
					moveAnim( Projectile.Face.LEFT );
				}
				face = Projectile.Face.LEFT;
				animFace = Projectile.Face.LEFT;
			}
			else if ( Input.GetKeyDown( KeyCode.D ) ) { //Right
				if ( attemptMove< Unit >( 1, 0 ) ) {
					moveAnim( Projectile.Face.RIGHT );
				}
				face = Projectile.Face.RIGHT;
				animFace = Projectile.Face.RIGHT;
			}
			else if ( Input.GetKeyDown( KeyCode.W ) ) { //Up
				if ( attemptMove< Unit >( 0, 1 ) ) {
					moveAnim( face );
				}
				face = Projectile.Face.UP;
			}
			else if ( Input.GetKeyDown( KeyCode.S ) ) { //Down
				if ( attemptMove< Unit >( 0, -1 ) ) {
					moveAnim( face );
				}
				face = Projectile.Face.DOWN;
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
				changeAnim( Projectile.Face.LEFT );
			}
			else if ( Input.GetKeyDown( KeyCode.RightArrow ) ) {
				changeAnim( Projectile.Face.RIGHT );
			}
			else if ( Input.GetKeyDown( KeyCode.UpArrow ) ) {
				changeAnim( Projectile.Face.UP );
			}
			else if ( Input.GetKeyDown( KeyCode.DownArrow ) ) {
				changeAnim( Projectile.Face.DOWN );
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

	void moveAnim( Projectile.Face nextState ) {
		if ( nextState == face || nextState == Projectile.Face.DOWN 
			|| nextState == Projectile.Face.UP ) {
			animator.SetTrigger( "Moving" );
		} else if ( nextState != animFace ) {
			animator.SetTrigger( "Swap" );
		}
	}

	void changeAnim( Projectile.Face nextState ) {
		if ( face != nextState ) {
			face = nextState;

			if ( face != Projectile.Face.DOWN && face != Projectile.Face.UP
				&& animFace != nextState ) {
				animator.SetTrigger( "Change" );
				animFace = nextState;
			}
		}
	}

	public int Health{ get; set; }
	public int Defense{ get; set; }
	public int Mana{ get; set; }
	public int ViewRange{ get; set; }
	public abstract void Hurt( int damage );

}