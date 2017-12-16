using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverScript : MonoBehaviour
{


	public float hoverDistance = 0.75f;
	public float hoverForce = 10.0f;
	public Rigidbody rb;
	private float currentHeight = 0.0f;
	private float hoverForceMultiplier = 0.0f;
	private Vector3 hoverForceApplied = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// -- Find hover distance and add force to make hover
		RaycastHit rayHit;
		if (Physics.Raycast(transform.position, Vector3.down, out rayHit))
		{
			currentHeight = rayHit.distance;
			if (currentHeight < (hoverDistance - Time.deltaTime))
			{
				// find percentage of currentHeight below Hoverdistance
				hoverForceMultiplier = (hoverDistance - currentHeight) / hoverDistance;
				hoverForceApplied = (Vector3.up * hoverForce * hoverForceMultiplier) + (Vector3.up * 9.8f);
				rb.AddForce(hoverForceApplied);
			}
			else
			{
				//Apply down Impulse force for half distance above hover
				if ((currentHeight - hoverDistance - Time.deltaTime) < (hoverDistance / 2))
				{
					hoverForceApplied = (Vector3.up * 9.8f) * ((hoverDistance - (currentHeight - hoverDistance)) / hoverDistance);
					rb.AddForce(hoverForceApplied);
				}
			}
		}
	}
}
