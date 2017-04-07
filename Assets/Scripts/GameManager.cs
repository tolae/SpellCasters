using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public GameObject character;
	public bool playerTurn;
	public bool TargetControl;
	private BoardManager boardScript;
	private float savedTime;
	private float currTime;

	// Use this for initialization
	void Awake () {
		if ( instance == null )
			instance = this;
		else if ( instance != null )
			Destroy( gameObject );

		DontDestroyOnLoad( gameObject );
		playerTurn = true;
		TargetControl = false;
		boardScript = GetComponent< BoardManager >();
		initGame();
	}

	void initGame() {
		int floor = 1;
		boardScript.instantiate( floor );
		character = boardScript.placePlayer( character );
		boardScript.placeEnemies( Random.Range( 1, 1 ) );
		transform.position = character.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if ( character.GetComponent< Player >().Health <= 0 ) {
			Debug.Log( "Game over! Player has fallen" );
			UnityEditor.EditorApplication.isPlaying = false;
		}
		transform.position = character.transform.position;
	}
}
