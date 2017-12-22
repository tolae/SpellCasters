using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {

	public LayerMask blockingLayer;
	public float moveTime = 0.2f;

	public int range;

	protected Rigidbody2D rb2d;
	protected float inverseMoveTime;

	protected virtual void Start() {
		rb2d = GetComponent< Rigidbody2D >();
		inverseMoveTime = 1f / moveTime;
	}

	public virtual void Cast( Vector3 start, EnumManager.Face face ) {
		Vector3 end = new Vector3( start.x, start.y, start.z );

		if ( face == EnumManager.Face.Up ) {
			end += new Vector3( 0, range * 1f, 0 );
		} else if ( face == EnumManager.Face.Down ) {
			end += new Vector3( 0, range * -1f, 0 );
		} else if ( face == EnumManager.Face.Left ) {
			end += new Vector3( range * -1f, 0, 0 );
		} else if ( face == EnumManager.Face.Right ) {
			end += new Vector3( range * 1f, 0, 0 );
		}

		RaycastHit2D hit = Physics2D.Linecast( start, end, blockingLayer );

		if ( hit.transform != null ) {
			StartCoroutine( move( hit.point, hit.transform ) );
		} else {
			StartCoroutine( move( end, null ) );
		}
	}

	protected IEnumerator move( Vector3 end, Transform obj ) {
		float remainingDist = ( transform.position - end ).sqrMagnitude;

		while ( remainingDist > 0 ) {
			Vector3 newPosition = Vector3.MoveTowards( rb2d.position, end, inverseMoveTime*Time.deltaTime );
			rb2d.MovePosition( newPosition );
			remainingDist = ( transform.position - end ).sqrMagnitude;
			if ( remainingDist <= 0 )
				onStop( obj );
			yield return null;
		}
	}

	protected virtual void onStop( Transform obj ) {
		if ( obj != null )
			obstructed( obj );
	}

	protected abstract void obstructed( Transform obj );
}