namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;
	public class Projectile : MonoBehaviour
	{
		public enum eProjectileState { idle, fired, hit };

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

		/// our parent tank's velocity at moment of firing (we add to fire velocity)
		public Vector3 mTankVelocity { get; protected set; }

		/// visible mesh for the projectile.
		public MeshRenderer mMesh { get; private set; }

		/// our particles for the projectile
		public ParticleSystem mParticles { get; protected set; }

		public float mKick { get; protected set; }
		protected virtual void Awake()
		{
			mKick = .003f;
			mParticles = GetComponentInChildren<ParticleSystem>();
			mMesh = GetComponentInChildren<MeshRenderer>();
			mTankVelocity = new Vector3(0, 0, 0);
			mTransform = transform;
			mCollider = GetComponentInChildren<CapsuleCollider>();
			mBody = GetComponentInChildren<Rigidbody>();
			SetState(eProjectileState.idle);
		}

		protected virtual void SetState(eProjectileState state)
		{
			mState = state;
			mStateTimer = 0.0f;
			///override actions in child classes.
		}

		protected virtual void Update() { UpdateState(); }
		protected virtual void UpdateState()
		{
			mStateTimer += Time.deltaTime;
			switch (mState)
			{
				case eProjectileState.idle:
					break;
				case eProjectileState.fired:
					{
						if (mStateTimer > mLifetime)
							SetState(eProjectileState.idle);
						break;
					}
				case eProjectileState.hit:
					break;
			}
		}

		/// fires the projectile. coroutine, as we need to wait a tick after repositioning the 
		/// projectile and setting it active.
		public virtual IEnumerator Fire(Transform projTransform, Vector3 tankVelocity)
		{
			mBody.position = projTransform.position;
			mBody.rotation = projTransform.rotation;
			mBody.isKinematic = false;
			mBody.velocity = tankVelocity;
			yield return new WaitForFixedUpdate();
			SetState(eProjectileState.fired);
		}

		protected virtual void OnCollisionEnter(Collision other)
		{
			if (mState == eProjectileState.fired)
				SetState(eProjectileState.hit);
		}
	}
}