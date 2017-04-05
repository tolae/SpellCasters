using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingUnit : MonoBehaviour {

	public float moveTime = .2f;
	public LayerMask blockingLayer;

	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent< BoxCollider2D >();
		rb2D = GetComponent< Rigidbody2D >();
		inverseMoveTime = 1f / moveTime;
	}

	protected bool move( int xDir, int yDir, out RaycastHit2D hit ) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2( xDir, yDir );

		boxCollider.enabled = false;
		hit = Physics2D.Linecast( start, end, blockingLayer );
		boxCollider.enabled = true;

		if ( hit.transform == null ) {
			StartCoroutine( smoothMovement( end ) );
			return true;
		}

		return false;
	}

	protected IEnumerator smoothMovement( Vector3 end ) {
		float sqrRemaningDistance = ( transform.position - end ).sqrMagnitude;

		while ( sqrRemaningDistance > 0 ) {
			Vector3 newPosition = Vector3.MoveTowards( rb2D.position, end, inverseMoveTime*Time.deltaTime );
			rb2D.MovePosition( newPosition );
			sqrRemaningDistance = ( transform.position - end ).sqrMagnitude;
			if ( !( sqrRemaningDistance > 0 ) )
				onStop();
			yield return null;
		}
	}

	protected virtual bool attemptMove< T > ( int xDir, int yDir ) 
		where T : Unit {
		RaycastHit2D hit;
		bool canMove = move( xDir, yDir, out hit );

		if ( hit.transform == null )
			return canMove;

		T hitComponenet = hit.transform.GetComponent< T >();

		if ( !canMove && hitComponenet != null ) {
			onCantMove( hitComponenet );
		}

		return canMove;
	}

	protected abstract void onCantMove< T > ( T component )
		where T : Unit;

	protected abstract void onStop();
}
