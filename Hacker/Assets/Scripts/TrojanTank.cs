namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;

	public class TrojanTank : MonoBehaviour
	{
		public Rigidbody mBody { get; private set; }
		public GameInput mGameInput { get; private set; }
		public HoverScript[] mHoverObjects { get; private set; }
		/// force multiplier for movement acceleration
		[SerializeField]
		private float mAccelerationForce = 1.0f;

		/// our max speed
		[SerializeField]
		private float mMaxSpeed = 5.0f;

		/// tank's transform
		public Transform mTransform { get; private set; }

		protected virtual void Awake()
		{
			mBody = GetComponent<Rigidbody>();
			mTransform = transform;
			mHoverObjects = GetComponentsInChildren<HoverScript>();
		}

		protected virtual void Start()
		{
			mGameInput = GameInput.mGameInput;
			mGameInput.ForwardIsDown += MoveForward;
		}

		protected virtual void MoveTank(Vector3 dir)
		{
			mBody.AddForce(mTransform.forward * mAccelerationForce, ForceMode.Acceleration);
		}

		////////////////////////////////////////////////
		/// our input event listeners://////////////////
		////////////////////////////////////////////////

		protected virtual void MoveForward(object source, EventArgs args)
		{
			if (mBody.velocity.magnitude < mMaxSpeed)
				MoveTank(Helpers.FlattenDir(mTransform.forward));
		}
	}
}