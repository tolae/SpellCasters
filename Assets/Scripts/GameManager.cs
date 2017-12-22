/**
 * Creator: Ethan Tola
 * 
 * This is the main controller for the game. GameManager is a static object this is 
 * instantiated once and never again. It will hold the camera and the character in
 * place will make everything referencable through the GameManager, giving other
 * classes access to everything else.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public GameObject character;
	public bool playerTurn;
	public bool TargetControl;
	public BoardManager boardScript;

	public List< GameObject > enemyStack = new List< GameObject >();
	public HashSet< GameObject > finishedStack = new HashSet< GameObject >();

	// Use this for initialization
	void Awake () {
		if ( instance == null )
			instance = this;
		else if ( instance != null )
			Destroy( gameObject );

		DontDestroyOnLoad( gameObject ); //Prevents this object from being destroyed
		playerTurn = true;
		TargetControl = false;
		boardScript = GetComponent< BoardManager >();
	}

	void Start() {
		initGame( 1 );
	}

	/**
	 * Initializes the game. Creates the board for the current floor
	*/
	void initGame( int floor ) {
		boardScript.instantiate( floor );
		character = boardScript.placePlayer( character );
		boardScript.placeEnemies();
		transform.position = character.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if ( character.GetComponent< Player >().Health <= 0 ) {
			Debug.Log( "Game over! Player has fallen" );
			UnityEditor.EditorApplication.isPlaying = false;
		}

		if ( !playerTurn ) {
			Debug.Log( "Enemy Stack Count: " + enemyStack.Count );
			if ( enemyStack.Count <= 0 ) {
				enemyStack.InsertRange( 0, finishedStack );
				playerTurn = true; 
			} else {
				enemyStack[ 0 ].GetComponent< Enemy >().enemyTurn();
				enemyStack.RemoveAt( 0 );
			}
		}

		transform.position = character.transform.position;
	}
}
