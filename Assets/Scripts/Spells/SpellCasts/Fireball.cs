using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : ProjSpell {

	void Awake() {
		Name = "Fireball";
		Description = "Sends a ball of concentrated forwards.";
		Damage = 10;
		Cost = 5;
		Range = 3;
	}

	public override void Cast (Vector3 start, EnumManager.Face face) {
		GetComponent< SpriteRenderer >().flipX = true;

		base.Cast (start, face);
	}
}
