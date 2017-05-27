using System.Collections;
using System.Collections.Generic;

public class EnumManager {

	//Used in BoardManager to determine the direction of hallway genereation
	public enum Pathfinder {
			Up,
			Down,
			Left,
			Right,
			None,
			Door,
	}
	//Used in Player to determine which direction the player is facing and which direction the
	//projectile spell is casted.
	public enum Face {
		Up,
		Down,
		Left,
		Right,
		None,
	}

	public enum PathStyle {
		Board,
		Unit,
	}
}
