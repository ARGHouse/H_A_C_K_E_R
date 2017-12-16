using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankImpulse : MonoBehaviour {

    public float Impulse = 5.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //We want player to move forward/back/left/right and atleast rotate

        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Impulse * Time.deltaTime);
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Impulse * Time.deltaTime);
    }
}
