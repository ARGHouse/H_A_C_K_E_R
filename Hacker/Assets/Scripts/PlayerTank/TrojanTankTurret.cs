namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using System;
	using UnityEngine.Networking;
	using UnityEngine;

	public class TrojanTankTurret : NetworkBehaviour
	{
		public Transform mTransform { get; protected set; }
		public Camera mCam { get; protected set; }

		/// offset of the camera from the turret.
		public Vector3 mCamOffset { get; protected set; }
		public GameInput mGameInput { get; protected set; }
		/// pivot point for the turret.
		public GameObject mPivot { get; protected set; }
		/// reference to the railgun
		public GameObject mRailgun { get; protected set; }
		/// pivot point for the railgun
		public GameObject mRailgunPivot { get; protected set; }

		/// fire point for projectiles
		public GameObject mMuzzle { get; protected set; }

		/// speed of our turret rotation
		[SerializeField]
		protected float mPivotSpeed = 10.0f;
		/// speed of the railgun rotation
		[SerializeField]
		protected float mRailgunPivotSpeed = 10.0f;
		public HackerGameManager mGame { get; protected set; }
		/// our reticule.
		public GameObject mReticule { get; protected set; }

		/// speed of our mouse  movement.
		[SerializeField]
		protected float mMouseHorizontalTurretSpeed = 1.0f;
		[SerializeField]
		protected float mMouseVerticalTurretSpeed = 1.0f;

		/// default distance for the reticule.
		[SerializeField]
		protected float mReticuleDefaultDistance = 50.0f;

		public TrojanTank mParentTank { get; protected set; }

		/// our projectiles
		public List<Projectile> mPrimaryProjectiles { get; protected set; }
		public List<Projectile> mSecondaryProjectiles { get; protected set; }

		protected GameObject mCameraPoint = null;

		public int mMaxProjectiles { get; protected set; }

		public ParticleSystem mMuzzleFlash { get; protected set; }
		protected virtual void Awake ()
		{
			mPrimaryProjectiles = new List<Projectile> ();
			mSecondaryProjectiles = new List<Projectile> ();

			mMaxProjectiles = 50;
			mTransform = transform;
			mParentTank = GetComponentInParent<TrojanTank> ();
			foreach (Component cp in GetComponentsInChildren<Transform> ())
			{
				if (cp.name == "PivotPoint")
					mPivot = cp.gameObject;
				if (cp.name == "Railgun")
					mRailgun = cp.gameObject;
				if (cp.name == "RailgunPivot")
					mRailgunPivot = cp.gameObject;
				if (cp.name == "MuzzlePoint")
					mMuzzle = cp.gameObject;
				if (cp.name == "MuzzleFlash")
					mMuzzleFlash = cp.GetComponentInChildren<ParticleSystem> ();
				if (cp.name == "CameraPoint")
					mCameraPoint = cp.gameObject;
			}

			mReticule = Instantiate (Helpers.LoadGameObjFromBundle ("TankAssets", "TESTReticule"), new Vector3 (10000, 10000, 10000), Quaternion.identity, this.transform.parent);
			mReticule.SetActive (false);

		}

		protected virtual void Start ()
		{
			if (mParentTank.isLocalPlayer)
			{
				mCam = mCameraPoint.gameObject.AddComponent<Camera> ();
				mCamOffset = mCam.transform.position - mTransform.position;
				GetComponentInParent<GameInput> ().SetCamera (mCam);
			}
			mGame = HackerGameManager.mGame;
			mGameInput = GameInput.mGameInput;
			mReticule.SetActive (true);

			if (!mParentTank.isLocalPlayer)
				return;
			mGameInput.FirePrimaryDown += FirePrimaryProjectile;
			mGameInput.FireSecondaryDown += FireSecondaryProjectile;

			/// we don't want it grabbing every mouse movement, so rotate differently for kb+m.
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

		void Update ()
		{
			if (!mParentTank.isLocalPlayer)
				return;

			Vector3 fwd = mRailgun.transform.forward;
			Vector3 pos = mMuzzle.transform.position;
			int layerMask = 1 << LayerMask.NameToLayer ("Default");
			RaycastHit hit;
			if (Physics.Raycast (pos, fwd, out hit, 100000, layerMask))
			{
				mReticule.transform.position = hit.point; // + new Vector3 (0, 1, 0);
			}
			else
				mReticule.transform.position = mRailgunPivot.transform.position + (mRailgun.transform.forward * mReticuleDefaultDistance);
		}

		private void MouseOrientTurret (object source, EventArgs args)
		{
			if (!mParentTank.isLocalPlayer)
				return;
			mTransform.RotateAround (mPivot.transform.position, mPivot.transform.up, mGameInput.mHorizontalRotation * Time.deltaTime * mMouseHorizontalTurretSpeed);
			mRailgun.transform.RotateAround (mRailgunPivot.transform.position, -mTransform.right, mGameInput.mVerticalRotation * Time.deltaTime * mMouseVerticalTurretSpeed);
		}

		protected virtual void RotateTurret (Vector3 dir)
		{
			if (!mParentTank.isLocalPlayer)
				return;
			mTransform.RotateAround (mPivot.transform.position, dir, mPivotSpeed);
		}

		protected virtual void RotateRailgun (Vector3 dir)
		{
			if (!mParentTank.isLocalPlayer)
				return;
			mRailgun.transform.RotateAround (mRailgunPivot.transform.position, dir, mRailgunPivotSpeed);
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

		protected virtual void FirePrimaryProjectile (object source, EventArgs args)
		{
			if (!mParentTank.isLocalPlayer)
				return;
			for (int i = 0; i < mPrimaryProjectiles.Count; i++)
			{
				if (mPrimaryProjectiles[i].mState == Projectile.eProjectileState.idle &&
					mPrimaryProjectiles[i].mStateTimer > 1.0f)
				{
					mMuzzleFlash.Play ();
					mPrimaryProjectiles[i].Fire (true);
					mParentTank.mBody.AddForceAtPosition (-mMuzzle.transform.forward * mPrimaryProjectiles[i].mFireForce * mPrimaryProjectiles[i].mKick, mMuzzle.transform.position, ForceMode.Impulse);
					break;
				}
			}
		}
		protected virtual void FireSecondaryProjectile (object source, EventArgs args)
		{
			if (!mParentTank.isLocalPlayer)
				return;
			print ("fire");
			for (int i = 0; i < mSecondaryProjectiles.Count; i++)
			{
				print ("fire2");
				if (mSecondaryProjectiles[i].mState == Projectile.eProjectileState.idle &&
					mSecondaryProjectiles[i].mStateTimer > 1.0f)
				{
					print ("fire3");
					mMuzzleFlash.Play ();
					mSecondaryProjectiles[i].Fire (false);
					mParentTank.mBody.AddForceAtPosition (-mMuzzle.transform.forward * mSecondaryProjectiles[i].mFireForce * mSecondaryProjectiles[i].mKick, mMuzzle.transform.position, ForceMode.Impulse);
					break;
				}
			}
		}
	}
}