﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MovingUnit, Unit {

	protected override void Start () {
		base.Start ();

		Health = 35;
		Defense = 0;
		Strength = 10;
		Mana = 0;
		ViewRange = 3;
	}

	protected void Update () {
		if ( GameManager.instance.playerTurn || GameManager.instance.TargetControl ) { return; }

		Vector3 playerPos = GameManager.instance.character.transform.position;

	}

	protected override bool attemptMove< T > (int xDir, int yDir) {
		bool canMove = base.attemptMove< T > (xDir, yDir);

		if ( canMove )
			GameManager.instance.playerTurn = true;

		return canMove;
	}

	protected override void onCantMove<T> (T component) {
		if ( component is Player ) {
			Attack( component );
			GameManager.instance.playerTurn = true;
		}

		GameManager.instance.playerTurn = false;
	}

	protected void Attack<T>( T notAlly ) where T : Unit {
		notAlly.Hurt( Strength );
	}

	public void Hurt ( int damage ) {
		int loss = damage - Defense;
		if ( loss < 0 )
			loss = 0;
		Health -= loss;
		Debug.Log( Health );
	}

	public int Health{ get; set; }
	public int Defense{ get; set; }
	public int Strength{ get; set; }
	public int Mana{ get; set; }
	public int ViewRange{ get; set; }

}
