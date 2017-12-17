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

		public Vector3 forwardd = new Vector3(0, 0, 0);
		/// force multiplier for movement acceleration
		[SerializeField]
		private float mAccelerationForce = 1.0f;

		/// our max speed
		[SerializeField]
		private float mMaxSpeed = 5.0f;

		[SerializeField]
		private float mTurnSpeed = 1.0f;

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
			mGameInput.BackwardIsDown += MoveBackward;
			mGameInput.LeftIsDown += TurnLeft;
			mGameInput.RightIsDown += TurnRight;
		}

		protected virtual void MoveTank(Vector3 dir)
		{
			forwardd = dir;
			mBody.AddForce(dir * mAccelerationForce, ForceMode.Acceleration);
		}

		////////////////////////////////////////////////
		/// our input event listeners://////////////////
		////////////////////////////////////////////////

		protected virtual void MoveForward(object source, EventArgs args)
		{
			if (mBody.velocity.magnitude < mMaxSpeed)
				MoveTank(Helpers.FlattenDir(mBody.transform.forward));
		}

		protected virtual void MoveBackward(object source, EventArgs args)
		{
			if (mBody.velocity.magnitude < mMaxSpeed)
				MoveTank(Helpers.FlattenDir(-mBody.transform.forward));
		}

		protected virtual void TurnLeft(object source, EventArgs args)
		{
			mBody.AddTorque(new Vector3(0, -mTurnSpeed, 0));
		}

		protected virtual void TurnRight(object source, EventArgs args)
		{
			mBody.AddTorque(new Vector3(0, mTurnSpeed, 0));
		}
	}
}