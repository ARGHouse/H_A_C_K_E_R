namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using System;
	using UnityEngine.Networking;
	using UnityEngine;

	public class TrojanTank : NetworkBehaviour
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

		public static TrojanTank mTank { get; private set; }

		///////////////////////////////////////////
		/// turret:////////////////////////////////
		///////////////////////////////////////////
		public Transform mTurretTransform { get; protected set; }
		public Camera mCam { get; protected set; }

		/// offset of the camera from the turret.
		public Vector3 mCamOffset { get; protected set; }
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

		/// our projectiles
		public List<Projectile> mPrimaryProjectiles { get; protected set; }
		public List<Projectile> mSecondaryProjectiles { get; protected set; }

		protected GameObject mCameraPoint = null;

		public int mMaxProjectiles { get; protected set; }

		public ParticleSystem mMuzzleFlash { get; protected set; }
		GameObject mBaseMissileProjObj = null;
		GameObject mMissileHitParticle = null;

		protected virtual void Awake ()
		{
			mBaseMissileProjObj = Helpers.LoadGameObjFromBundle ("TankAssets", "Missile");
			mMissileHitParticle = Helpers.LoadGameObjFromBundle ("TankAssets", "MissileExplosion");
			mTank = this;
			mBody = GetComponent<Rigidbody> ();
			mTransform = transform;
			mHoverObjects = GetComponentsInChildren<HoverScript> ();

			FindChildren ();

			mPrimaryProjectiles = new List<Projectile> ();
			mSecondaryProjectiles = new List<Projectile> ();

			mMaxProjectiles = 50;

			mReticule = Instantiate (Helpers.LoadGameObjFromBundle ("TankAssets", "TESTReticule"), new Vector3 (10000, 10000, 10000), Quaternion.identity, this.transform.parent);
			mReticule.SetActive (false);
		}

		protected virtual void FindChildren ()
		{
			foreach (Component cp in GetComponentsInChildren<Transform> ())
			{
				if (cp.name == "LeftThrottlePoint")
					mLeftThrottlePoint = cp.transform;
				else if (cp.name == "RightThrottlePoint")
					mRightThrottlePoint = cp.transform;
				else if (cp.name == "PivotPoint")
					mPivot = cp.gameObject;
				else if (cp.name == "RailgunParent")
					mRailgun = cp.gameObject;
				else if (cp.name == "RailgunPivot")
					mRailgunPivot = cp.gameObject;
				else if (cp.name == "MuzzlePoint")
					mMuzzle = cp.gameObject;
				else if (cp.name == "MuzzleFlash")
					mMuzzleFlash = cp.GetComponentInChildren<ParticleSystem> ();
				else if (cp.name == "CameraPoint")
					mCameraPoint = cp.gameObject;
				else if(cp.name == "TurretParent")
					mTurretTransform = cp.transform;
			}
		}

		protected virtual void Start ()
		{
			TurretStart ();

			if (!isLocalPlayer)
				return;
			mGameInput = GameInput.mGameInput;
			mGameInput.LeftThrottleIsDown += LeftThrottle;
			mGameInput.RightThrottleIsDown += RightThrottle;
			CmdSpawnLocalComponents ();
		}

		protected virtual void TurretStart ()
		{

			mGame = HackerGameManager.mGame;
			mGameInput = GameInput.mGameInput;
			mReticule.SetActive (true);

			if (!isLocalPlayer)
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
		public override void OnStartLocalPlayer ()
		{
			if (isLocalPlayer)
			{
				mCam = mCameraPoint.gameObject.AddComponent<Camera> ();
				mCamOffset = mCam.transform.position - mTransform.position;
				GetComponentInParent<GameInput> ().SetCamera (mCam);
			}
		}

		void Update ()
		{
			if (!isLocalPlayer)
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

		[Command]
		public void CmdSpawnLocalComponents ()
		{
			print ("spawned");

			/// create our projectiles.
			GameObject baseLaserProjObj = Helpers.LoadGameObjFromBundle ("TankAssets", "LaserProjectile");
			/// so as not to muddy the editor.
			GameObject go = Instantiate (new GameObject (), new Vector3 (0, 0, 0), Quaternion.identity);
			go.name = "PrimaryProjectiles";
			GameObject laserhitParticle = Helpers.LoadGameObjFromBundle ("TankAssets", "Spark");
			for (int i = 0; i < mMaxProjectiles; i++)
			{
				mPrimaryProjectiles.Add (Instantiate (baseLaserProjObj, mTransform.position, Quaternion.identity, go.transform).GetComponentInChildren<Projectile> ());
				mPrimaryProjectiles[mPrimaryProjectiles.Count - 1].Init (this, laserhitParticle);
			}
		}

		protected virtual void MoveTank (Vector3 dir)
		{
			if (!isLocalPlayer)
				return;
			mBody.AddForce (dir * mAccelerationForce, ForceMode.Acceleration);
		}

		protected virtual void LeftThrottle (object source, EventArgs args)
		{
			if (!isLocalPlayer)
				return;
			Vector3 dir = Helpers.FlattenDir (mBody.transform.forward);
			mBody.AddForceAtPosition (dir * mGameInput.mLeftThrottle * mThrottleForce, mLeftThrottlePoint.position, ForceMode.Acceleration);
		}

		protected virtual void RightThrottle (object source, EventArgs args)
		{
			if (!isLocalPlayer)
				return;
			Vector3 dir = Helpers.FlattenDir (mBody.transform.forward);
			mBody.AddForceAtPosition (dir * mGameInput.mRightThrottle * mThrottleForce, mRightThrottlePoint.position, ForceMode.Acceleration);
		}

		private void MouseOrientTurret (object source, EventArgs args)
		{
			if (!isLocalPlayer)
				return;
			mTransform.RotateAround (mPivot.transform.position, mPivot.transform.up, mGameInput.mHorizontalRotation * Time.deltaTime * mMouseHorizontalTurretSpeed);
			mRailgun.transform.RotateAround (mRailgunPivot.transform.position, -mTransform.right, mGameInput.mVerticalRotation * Time.deltaTime * mMouseVerticalTurretSpeed);
		}

		protected virtual void RotateTurret (Vector3 dir)
		{
			if (!isLocalPlayer)
				return;
			mTurretTransform.RotateAround (mPivot.transform.position, dir, mPivotSpeed);
			//mRailgun.transform.RotateAround (mPivot.transform.position, dir, mPivotSpeed);
		}

		protected virtual void RotateRailgun (Vector3 dir)
		{
			if (!isLocalPlayer)
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
			RotateRailgun (mTurretTransform.right);
		}
		protected virtual void RotateDown (object source, EventArgs args)
		{
			RotateRailgun (-mTurretTransform.right);
		}

		protected virtual void FirePrimaryProjectile (object source, EventArgs args)
		{
			if (!isLocalPlayer)
				return;
			for (int i = 0; i < mPrimaryProjectiles.Count; i++)
			{
				if (mPrimaryProjectiles[i].mState == Projectile.eProjectileState.idle &&
					mPrimaryProjectiles[i].mStateTimer > 1.0f)
				{
					mMuzzleFlash.Play ();
					mPrimaryProjectiles[i].Fire (true);
					mBody.AddForceAtPosition (-mMuzzle.transform.forward * mPrimaryProjectiles[i].mFireForce * mPrimaryProjectiles[i].mKick, mMuzzle.transform.position, ForceMode.Impulse);
					break;
				}
			}
		}
		protected virtual void FireSecondaryProjectile (object source, EventArgs args)
		{
			if (!isLocalPlayer)
				return;
			CmdFireSecondary ();
		}

		[Command]
		protected virtual void CmdFireSecondary ()
		{
			MissileProjectile projectile = Instantiate (mBaseMissileProjObj, mMuzzle.transform.position, mMuzzle.transform.rotation).GetComponent<MissileProjectile> ();
			projectile.Init (this, mMissileHitParticle);
			projectile.transform.forward = mRailgun.transform.forward;
			mMuzzleFlash.Play ();
			projectile.GetComponent<Projectile> ().Fire (false);
			mBody.AddForceAtPosition (-mMuzzle.transform.forward * projectile.mFireForce * projectile.mKick, mMuzzle.transform.position, ForceMode.Impulse);
			NetworkServer.Spawn (projectile.gameObject);
		}
	}
}