namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;

	public class TrojanTankTurret : MonoBehaviour
	{
		public Transform mTransform { get; private set; }
		public Camera mCam { get; private set; }
		public Vector3 mCamOffset { get; private set; }
		[SerializeField]
		public GameInput mGameInput { get; private set; }
		public GameObject mPivot { get; private set; }
		public GameObject mRailgun { get; private set; }
		public GameObject mRailgunPivot { get; private set; }
		[SerializeField]
		private float mPivotSpeed = 10.0f;
		[SerializeField]
		private float mRailgunPivotSpeed = 10.0f;
		public HackerGameManager mGame { get; private set; }
		public GameObject mReticule { get; private set; }
		public Vector3 mRotateStartPos { get; private set; }
		[SerializeField]
		public float mMouseTurretSpeed = 1.0f;

		protected virtual void Awake()
		{
			mCam = Camera.main;
			mTransform = transform;
			mCamOffset = mCam.transform.position - mTransform.position;
			foreach (Component cp in GetComponentsInChildren<Transform>())
			{
				if (cp.name == "PivotPoint")
					mPivot = cp.gameObject;
				if (cp.name == "Railgun")
					mRailgun = cp.gameObject;
				if (cp.name == "RailgunPivot")
					mRailgunPivot = cp.gameObject;
			}
			mReticule = Instantiate(Helpers.LoadGameObjFromBundle("TankAssets", "TESTReticule"), new Vector3(0, 0, 0), Quaternion.identity, this.transform.parent);
			mReticule.SetActive(false);
			mRotateStartPos = new Vector3(0,0,0);
		}

		protected virtual void Start()
		{
			mGame = HackerGameManager.mGame;
			mGameInput = GameInput.mGameInput;
			if (mGame.UseController)
			{
				mGameInput.RotateRightIsDown += RotateRight;
				mGameInput.RotateLeftIsDown += RotateLeft;
				mGameInput.RotateUpIsDown += RotateUp;
				mGameInput.RotateDownIsDown += RotateDown;
			}
			else
			{
				mReticule.SetActive(true);
				mGameInput.RightClickIsDown += MouseOrientTurret;
			}
		}

		private void MouseOrientTurret(object source, EventArgs args)
		{
			if (mGame.UseController)
				return;
			mTransform.RotateAround(mPivot.transform.position, mPivot.transform.up, mGameInput.mMouseHorizontal * Time.deltaTime * mMouseTurretSpeed);
		}

		protected virtual void RotateTurret(Vector3 dir)
		{
			mTransform.RotateAround(mPivot.transform.position, dir, mPivotSpeed);
		}

		protected virtual void RotateRailgun(Vector3 dir)
		{
			mRailgun.transform.RotateAround(mRailgunPivot.transform.position, dir, mRailgunPivotSpeed);
		}

		protected virtual void RotateRight(object source, EventArgs args)
		{
			RotateTurret(mTransform.up);
		}

		protected virtual void RotateLeft(object source, EventArgs args)
		{
			RotateTurret(-mTransform.up);
		}

		protected virtual void RotateUp(object source, EventArgs args)
		{
			RotateRailgun(mTransform.right);
		}
		protected virtual void RotateDown(object source, EventArgs args)
		{
			RotateRailgun(-mTransform.right);
		}
	}
}