using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingUnit : MonoBehaviour, Unit {

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

		while ( sqrRemaningDistance > 0f ) {
			Vector3 newPosition = Vector3.MoveTowards( rb2D.position, end, inverseMoveTime*Time.deltaTime );
			rb2D.MovePosition( newPosition );
			sqrRemaningDistance = ( transform.position - end ).sqrMagnitude;
			if ( sqrRemaningDistance <= 0f ) {
				onStop();
				yield return null;
			}
			yield return null;
		}
	}

	protected virtual bool Detect< T >( Vector3 start, Vector3 end ) 
		where T : Unit {

		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast( start, end, blockingLayer );
		boxCollider.enabled = true;

		if ( hit.transform == null )
			return false;

		T hitComponent = hit.transform.GetComponent< T >();

		if ( hitComponent != null && Mathf.Abs( start.magnitude - end.magnitude ) <= ViewRange )
			return unitDetect( hitComponent );

		return false;
	}

	protected abstract bool unitDetect< T >( T component ) 
		where T : Unit;

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

	//Simple calculation between two objects.
	public virtual float distFrom( GameObject go ) {
		float myX = transform.position.x;
		float myY = transform.position.y;
		float otherX = go.transform.position.x;
		float otherY = go.transform.position.y;

		float dist = Mathf.Sqrt( Mathf.Pow( myX - otherX, 2) + Mathf.Pow( myY - otherY, 2 ) );

		return dist;
	}

	protected abstract void onCantMove< T > ( T component )
		where T : Unit;

	protected abstract void onStop();

	public int Health{ get; set; }
	public int Defense{ get; set; }
	public int Mana{ get; set; }
	public int ViewRange{ get; set; }
	public abstract void Hurt( int damage );
}
