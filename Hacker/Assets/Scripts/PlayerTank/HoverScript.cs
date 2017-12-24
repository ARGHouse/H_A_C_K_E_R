namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class HoverScript : MonoBehaviour
	{
		/// distance we'll hover above the ground
		public float hoverDistance = 0.75f;
		/// strength of the hover force
		public float hoverForce = 10.0f;
		/// reference to our rigidbody
		public Rigidbody rb;
		/// our current height
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
			Vector3 offset = new Vector3(0, 100, 0); //just to make sure we haven't ended up below the ground mesh somehow...
			int layerMask = 1 << LayerMask.NameToLayer("Default");
			if (Physics.Raycast(transform.position + offset, -Vector3.up, out rayHit, 1000, layerMask))
			{
				currentHeight = rayHit.distance - offset.y;
				if (currentHeight < (hoverDistance))
				{
					// find percentage of currentHeight below Hoverdistance
					hoverForceMultiplier = (hoverDistance - currentHeight) / hoverDistance;
					hoverForceApplied = (transform.up * hoverForce * hoverForceMultiplier) - (Physics.gravity / 4);
					rb.AddForceAtPosition(hoverForceApplied, transform.position, ForceMode.Force);
				}
				else
				{
					//Apply down Impulse force for half distance above hover
					if (currentHeight >(hoverDistance * 1.5f))
					{
						hoverForceApplied = Physics.gravity / 4;
						rb.AddForceAtPosition(hoverForceApplied, transform.position, ForceMode.Force);
					}
				}
			}
		}
	}
}