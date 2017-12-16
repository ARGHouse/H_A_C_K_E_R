using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //This is where our player will start when the game is played
        //Player is to start at x 0 y -0.8 and z 0

        transform.position = new Vector3(0, 3.59f, 0);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
