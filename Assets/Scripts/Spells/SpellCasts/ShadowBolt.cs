using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBolt : ProjSpell {

	void Awake() {
		Name = "Shadow Bolt";
		Description = "Gather dark energy around you and send it forward.";
		Damage = 10;
		Cost = 10;
		Range = 2;
	}

	public override void Cast (Vector3 start, EnumManager.Face face) {
		GetComponent< SpriteRenderer >().flipX = false;

		base.Cast (start, face);
	}
}
