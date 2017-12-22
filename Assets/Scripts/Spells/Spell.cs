using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spell {

	void Cast( Vector3 start, EnumManager.Face face );

	string Name{ get; set; }
	string Description{ get; set; }

	int Damage{ get; set; }
	int Cost{ get; set; }
	int Range{ get; set; }

}
