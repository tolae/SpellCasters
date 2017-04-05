using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : TargetSpell {

	void Awake() {
		Name = "Lightning";
		Description = "Call down thunder and lightning from the heavens to strike target enemy.";
		Damage = 15;
		Cost = 10;
		Range = 3;
		AoE = 1;
	}

	protected override void Start () {
		base.Start ();

		range = Range;
	}

}
