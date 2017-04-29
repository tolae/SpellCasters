using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTim : Player {

	protected override void Start () {
		base.Start ();
		base.Health = 120;
		base.Defense = -5;
		base.Mana = 10;
		base.ViewRange = 4;
	}

	public override void Hurt ( int damage ) {
		int loss = damage - Defense;
		if ( loss < 0 )
			loss = 0;
		Health -= loss;
		Debug.Log( Health );
	}

}