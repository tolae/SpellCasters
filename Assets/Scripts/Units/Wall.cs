using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, Unit {

	public Sprite destroyed;

	new SpriteRenderer renderer;
	void Awake () {
		renderer = GetComponent< SpriteRenderer >();
		Health = 100;
		Defense = int.MaxValue;
		Mana = 0;
		ViewRange = 0;
	}

	public void Hurt( int damage ) {
		int loss = damage - Defense;
		if ( loss < 0 )
			loss = 0;
		Health -= loss;
		Debug.Log( Health );
		if ( Health <= 0 ) {
			renderer.sprite = this.destroyed;
			gameObject.SetActive( false );
		}
	}

	public float distFrom( GameObject go ) { return -1f; } //Walls don't do much

	public int Health{ get; set; }
	public int Defense{ get; set; }
	public int Mana{ get; set; }
	public int ViewRange{ get; set; }

}
