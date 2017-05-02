using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpot : GridSpot {

	private Room parent;

	public RoomSpot ( Vector3 coord, GameObject obj, bool changeable, Room parentRoom ) : base(coord, obj, changeable) {
		this.parent = parentRoom;
	}

	public Room parentRoom() {
		return parent;
	}
}