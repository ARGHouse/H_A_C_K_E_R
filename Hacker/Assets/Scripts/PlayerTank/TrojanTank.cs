namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using System;
	using UnityEngine;

	public class TrojanTank : MonoBehaviour
	{
		public Rigidbody mBody { get; private set; }
		public GameInput mGameInput { get; private set; }
		public HoverScript[] mHoverObjects { get; private set; }

		/// force multiplier for movement acceleration when not using controller triggers
		[SerializeField]
		private float mAccelerationForce = 1.0f;

		/// when using the throttle scheme (controller triggers)
		[SerializeField]
		private float mThrottleForce = 1.0f;

		/// tank's transform
		public Transform mTransform { get; private set; }
		/// point from which we'll apply force for movement
		public Transform mLeftThrottlePoint { get; private set; }
		public Transform mRightThrottlePoint { get; private set; }

		/// reference to our turret
		public TrojanTankTurret mTurret { get; private set; }

		protected virtual void Awake()
		{
			mTurret = GetComponentInChildren<TrojanTankTurret>();
			mBody = GetComponent<Rigidbody>();
			mTransform = transform;
			mHoverObjects = GetComponentsInChildren<HoverScript>();
			foreach (Component cp in GetComponentsInChildren<Transform>())
			{
				if (cp.name == "LeftThrottlePoint")
					mLeftThrottlePoint = cp.transform;
				else if (cp.name == "RightThrottlePoint")
					mRightThrottlePoint = cp.transform;
			}
		}

		protected virtual void Start()
		{
			mGameInput = GameInput.mGameInput;
			mGameInput.LeftThrottleIsDown += LeftThrottle;
			mGameInput.RightThrottleIsDown += RightThrottle;
		}

		protected virtual void MoveTank(Vector3 dir)
		{
			mBody.AddForce(dir * mAccelerationForce, ForceMode.Acceleration);
		}

		protected virtual void LeftThrottle(object source, EventArgs args)
		{
			Vector3 dir = Helpers.FlattenDir(mBody.transform.forward);
			mBody.AddForceAtPosition(dir * mGameInput.mLeftThrottle * mThrottleForce, mLeftThrottlePoint.position, ForceMode.Acceleration);
		}

		protected virtual void RightThrottle(object source, EventArgs args)
		{
			Vector3 dir = Helpers.FlattenDir(mBody.transform.forward);
			mBody.AddForceAtPosition(dir * mGameInput.mRightThrottle * mThrottleForce, mRightThrottlePoint.position, ForceMode.Acceleration);
		}
	}
}