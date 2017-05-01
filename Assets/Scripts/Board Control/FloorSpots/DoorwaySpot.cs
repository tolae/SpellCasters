using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwaySpot : GridSpot {

	public bool isConnected{ get; private set; }

	public DoorwaySpot ( Vector3 coord, GameObject obj, bool changeable ) : base (coord, obj, changeable) {
		isConnected = false;
	}

}
