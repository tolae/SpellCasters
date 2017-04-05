using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpell : Targeting, Spell {

	protected override void Start () {
		base.Start ();

		gameObject.SetActive( false );
	}

	public override void Cast (Vector3 start, Projectile.Face face) {
		gameObject.SetActive( true );
		GetComponent< SpriteRenderer >().enabled = false;
		GetComponent< Animator >().enabled = false;
		base.Cast (start, face);
	}

	protected override void Update() {
		base.Update();

		if ( Input.GetKeyDown( KeyCode.Escape ) )
			CancelCast();

		if ( GetComponent< Animator >().GetCurrentAnimatorStateInfo(0).IsName( Name + " Anim" )
			&& GetComponent< Animator >().enabled == true )
			Finished();
	}
		
	public void CancelCast() {
		GameManager.instance.TargetControl = false;
		GameManager.instance.playerTurn = true;
		GameManager.instance.character.GetComponent< Player >().Mana += Cost;
		foreach ( GameObject t in this.tiles )
			Destroy( t );
	}

	public void Finished() {
		GameManager.instance.playerTurn = false;
		GetComponent< Animator >().enabled = false;
	}

	protected override void ActualCast ( Vector3 start ) {
		GetComponent< SpriteRenderer >().enabled = true;
		GetComponent< Animator >().enabled = true;

		base.ActualCast (start);
	}

	public string Name{ get; set; }
	public string Description{ get; set; }
	public int Damage{ get; set; }
	public int Cost{ get; set; }
	public int Range{ get; set; }
	new public int AoE{ get; set; }

}
