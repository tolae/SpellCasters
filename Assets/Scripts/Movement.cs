using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown( KeyCode.W ) )
			transform.position += new Vector3( 0f, 1f, 0f );
		else if ( Input.GetKeyDown( KeyCode.S ) )
			transform.position += new Vector3( 0f, -1f, 0f );
		else if ( Input.GetKeyDown( KeyCode.A ) )
			transform.position += new Vector3( -1f, 0f, 0f );
		else if ( Input.GetKeyDown( KeyCode.D ) )
			transform.position += new Vector3( 1f, 0f, 0f );
	}
}
