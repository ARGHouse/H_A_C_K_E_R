namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using System;
	using UnityEngine;

	public class TrojanTankTurret : MonoBehaviour
	{
		public Transform mTransform { get; private set; }
		public Camera mCam { get; private set; }

		/// offset of the camera from the turret.
		public Vector3 mCamOffset { get; private set; }
		public GameInput mGameInput { get; private set; }
		/// pivot point for the turret.
		public GameObject mPivot { get; private set; }
		/// reference to the railgun
		public GameObject mRailgun { get; private set; }
		/// pivot point for the railgun
		public GameObject mRailgunPivot { get; private set; }
		/// speed of our turret rotation
		[SerializeField]
		private float mPivotSpeed = 10.0f;
		/// speed of the railgun rotation
		[SerializeField]
		private float mRailgunPivotSpeed = 10.0f;
		public HackerGameManager mGame { get; private set; }
		/// our reticule.
		public GameObject mReticule { get; private set; }

		/// speed of our mouse  movement.
		[SerializeField]
		private float mMouseHorizontalTurretSpeed = 1.0f;
		[SerializeField]
		private float mMouseVerticalTurretSpeed = 1.0f;

		/// default distance for the reticule.
		[SerializeField]
		private float mReticuleDefaultDistance = 50.0f;

		protected virtual void Awake ()
		{
			mCam = Camera.main;
			mTransform = transform;
			mCamOffset = mCam.transform.position - mTransform.position;

			foreach (Component cp in GetComponentsInChildren<Transform> ())
			{
				if (cp.name == "PivotPoint")
					mPivot = cp.gameObject;
				if (cp.name == "Railgun")
					mRailgun = cp.gameObject;
				if (cp.name == "RailgunPivot")
					mRailgunPivot = cp.gameObject;
			}
			mReticule = Instantiate (Helpers.LoadGameObjFromBundle ("TankAssets", "TESTReticule"), new Vector3 (0, 0, 0), Quaternion.identity, this.transform.parent);
			mReticule.SetActive (false);

		}

		protected virtual void Start ()
		{
			mGame = HackerGameManager.mGame;
			mGameInput = GameInput.mGameInput;
			mReticule.SetActive (true);

			if (mGame.UseController)
			{
				mGameInput.RotateRightIsDown += RotateRight;
				mGameInput.RotateLeftIsDown += RotateLeft;
				mGameInput.RotateUpIsDown += RotateUp;
				mGameInput.RotateDownIsDown += RotateDown;
			}
			else
				mGameInput.RightClickIsDown += MouseOrientTurret;
		}

		private void MouseOrientTurret (object source, EventArgs args)
		{
			mTransform.RotateAround (mPivot.transform.position, mPivot.transform.up, mGameInput.mMouseHorizontal * Time.deltaTime * mMouseHorizontalTurretSpeed);
			mRailgun.transform.RotateAround (mRailgunPivot.transform.position, -mTransform.right, mGameInput.mMouseVertical * Time.deltaTime * mMouseVerticalTurretSpeed);
			mReticule.transform.position = mRailgunPivot.transform.position + (mRailgun.transform.forward * 40);
		}

		protected virtual void RotateTurret (Vector3 dir)
		{
			mTransform.RotateAround (mPivot.transform.position, dir, mPivotSpeed);
			mReticule.transform.position = mRailgunPivot.transform.position + (mRailgun.transform.forward * 40);
		}

		protected virtual void RotateRailgun (Vector3 dir)
		{
			mRailgun.transform.RotateAround (mRailgunPivot.transform.position, dir, mRailgunPivotSpeed);
			mReticule.transform.position = mRailgunPivot.transform.position + (mRailgun.transform.forward * 40);
		}

		protected virtual void RotateRight (object source, EventArgs args)
		{
			RotateTurret (mTransform.up);
		}

		protected virtual void RotateLeft (object source, EventArgs args)
		{
			RotateTurret (-mTransform.up);
		}

		protected virtual void RotateUp (object source, EventArgs args)
		{
			RotateRailgun (mTransform.right);
		}
		protected virtual void RotateDown (object source, EventArgs args)
		{
			RotateRailgun (-mTransform.right);
		}
	}
}