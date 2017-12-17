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
	void Awake()
	{
		rb = GetComponentInParent<Rigidbody>();
	}

	void FixedUpdate()
	{
		RaycastHit rayHit;
		Vector3 offset = new Vector3(0, 100, 0);//just to make sure we haven't ended up below the ground mesh somehow...
		int layerMask = 1 << LayerMask.NameToLayer("Default");
		if (Physics.Raycast(transform.position + offset, -Vector3.up, out rayHit, 1000, layerMask))
		{
			currentHeight = rayHit.distance - offset.y;
			if (currentHeight < (hoverDistance))
			{
				// find percentage of currentHeight below Hoverdistance
				hoverForceMultiplier = (hoverDistance - currentHeight) / hoverDistance;
				hoverForceApplied = (Vector3.up * hoverForce * hoverForceMultiplier) + Physics.gravity;
				rb.AddForceAtPosition(hoverForceApplied, transform.position, ForceMode.Force);
			}
			else
			{
				//Apply down Impulse force for half distance above hover
				if ((currentHeight - hoverDistance ) < (hoverDistance / 2))
				{
					hoverForceApplied = Physics.gravity * ((hoverDistance - (currentHeight - hoverDistance)) / hoverDistance);
					rb.AddForceAtPosition(hoverForceApplied, transform.position, ForceMode.Force);
				}
			}
		}
	}
}
