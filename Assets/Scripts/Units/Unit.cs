using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Unit {

	int Health{ get; set; }
	int Defense{ get; set; }
	int Mana{ get; set; }
	int ViewRange{ get; set; }
	void Hurt( int damage );

}