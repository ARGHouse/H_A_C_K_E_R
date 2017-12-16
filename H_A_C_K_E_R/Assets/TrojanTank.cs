namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TrojanTank : MonoBehaviour
	{
		public Rigidbody mBody { get; private set; }

		protected virtual void Awake()
		{
			mBody = GetComponent<Rigidbody>();
		}
	}
}