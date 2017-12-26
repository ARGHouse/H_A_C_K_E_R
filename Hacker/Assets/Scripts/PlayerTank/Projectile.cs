namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;
	public class Projectile : MonoBehaviour
	{
		public enum eProjectileState { idle, fired, hit };

		public GameInput mInput { get; protected set; }
		public eProjectileState mState { get; protected set; }
		public float mStateTimer { get; protected set; }

		/// how long this will stay alive after being fired.
		[SerializeField]
		protected float mLifetime = 3.0f;

		public Transform mTransform { get; protected set; }
		public Rigidbody mBody { get; protected set; }

		/// force multiplier for firing projectile.
		public float mFireForce = 10.0f;

		public CapsuleCollider mCollider { get; protected set; }

		/// visible mesh for the projectile.
		public MeshRenderer mMesh { get; private set; }

		/// our particles for the projectile
		public ParticleSystem mParticles { get; protected set; }
		public GameObject mHitParticle = null;

		public float mKick { get; protected set; }
		public TrojanTank mTankParent { get; protected set; }

		protected virtual void Awake ()
		{
			mKick = .003f;
			mParticles = GetComponentInChildren<ParticleSystem> ();
			mMesh = GetComponentInChildren<MeshRenderer> ();
			mTransform = transform;
			mCollider = GetComponentInChildren<CapsuleCollider> ();
			mBody = GetComponentInChildren<Rigidbody> ();
			SetState (eProjectileState.idle);
		}
		public virtual void Init (TrojanTank tank, GameObject hitParticle)
		{
			mTankParent = tank;
			mHitParticle = hitParticle;
		}

		protected virtual void Start ()
		{
			mInput = GameInput.mGameInput;
		}
		protected virtual void SetState (eProjectileState state)
		{
			mState = state;
			mStateTimer = 0.0f;
			///override actions in child classes.
		}

		protected virtual void Update () { UpdateState (); }
		protected virtual void UpdateState ()
		{
			mStateTimer += Time.deltaTime;
			switch (mState)
			{
				case eProjectileState.idle:
					break;
				case eProjectileState.fired:
					{
						if (mStateTimer > mLifetime)
							SetState (eProjectileState.idle);
						break;
					}
				case eProjectileState.hit:
					break;
			}
		}

		/// fires the projectile. coroutine, as we need to wait a tick after repositioning the 
		/// projectile and setting it active.
		public virtual IEnumerator Fire (Transform projTransform, bool useParentVelocity = true)
		{
			mBody.position = projTransform.position;
			mBody.rotation = projTransform.rotation;
			mBody.isKinematic = false;
			if(useParentVelocity)
				mBody.velocity = mTankParent.mBody.velocity;
			else 
				mBody.velocity = new Vector3(0,0,0);
			yield return new WaitForFixedUpdate ();
			SetState (eProjectileState.fired);
		}

		protected virtual void OnCollisionEnter (Collision other)
		{
			if (mState == eProjectileState.fired)
				SetState (eProjectileState.hit);
		}
	}
}