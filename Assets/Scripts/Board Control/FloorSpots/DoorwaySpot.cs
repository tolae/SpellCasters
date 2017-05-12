using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwaySpot : RoomSpot {

	public bool isConnected{ get; private set; }

	public DoorwaySpot ( Vector3 coord, GameObject obj, bool changeable, Room parent ) : 
		base (coord, obj, changeable, parent ) {
		isConnected = false;
	}

}
