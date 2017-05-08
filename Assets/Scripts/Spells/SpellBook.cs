using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour {

	public GameObject[] spells;

	private GameObject currSpell;
	private int spellIndex;

	void Start() {
		currSpell = (GameObject) Instantiate( spells[ 0 ], transform.position, Quaternion.identity );
		spellIndex = 0;
	}

	public void castFirst( Vector3 start, EnumManager.Face face ) {
		GetComponentInParent< Player >().Mana -= currSpell.GetComponent< Spell >().Cost;
		if ( GetComponentInParent< Player >().Mana < 0 ) {
			Debug.Log( "Unable to cast spell! Need more mana!" );
			GetComponentInParent< Player >().Mana += currSpell.GetComponent< Spell >().Cost;
			return;
		}
		currSpell.transform.position = transform.position;
		currSpell.GetComponent< Spell >().Cast( start, face );
		Debug.Log( "Current Mana: " + this.GetComponentInParent< Player >().Mana );
	}

	public void nextSpell() {
		Destroy( currSpell );

		spellIndex++;

		currSpell = (GameObject) Instantiate( spells[ spellIndex % spells.Length ], 
			transform.position, Quaternion.identity );

		Debug.Log( "Spell alternated. New spell " + currSpell.GetComponent< Spell >().Name );
	}
}