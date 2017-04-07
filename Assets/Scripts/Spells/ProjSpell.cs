using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjSpell : Projectile, Spell {

	public bool NEED_FLIP = true;
	public bool NEED_TARGET = false;

	protected override void Start () {
		base.Start ();

		gameObject.SetActive( false );
		this.range = Range;
		Debug.Log( "Spell started" );
	}

	public override void Cast( Vector3 start, Face face ) {
		gameObject.SetActive( true );

		if ( face == Face.LEFT && NEED_FLIP )
			GetComponent< SpriteRenderer >().flipX = !( GetComponent< SpriteRenderer >().flipX );

		base.Cast(start, face);
	}

	protected override void onStop( Transform obj ) {
		base.onStop( obj );
		transform.position = GameManager.instance.character.transform.position;
		gameObject.SetActive( false );
	}

	protected override void obstructed( Transform obj ) {
		Debug.Log( "Hit something" );
		obj.GetComponent< Unit >().Hurt( Damage );
	}

	public string Name{ get; set; }
	public string Description{ get; set; }
	public int Damage{ get; set; }
	public int Cost{ get; set; }
	public int Range{ get; set; }
}