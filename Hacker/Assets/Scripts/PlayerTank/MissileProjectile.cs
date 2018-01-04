namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine.Networking;
	using UnityEngine;
	public class MissileProjectile : Projectile
	{
		/// amount above the target we'll max the arc at.
		[SerializeField]
		private float mTrajectoryApexOffset = 10.0f;
		protected override void Awake ()
		{
			base.Awake ();
		}
		protected override void SetState (eProjectileState state)
		{
			base.SetState (state);
			switch (mState)
			{
				case eProjectileState.idle:
					mParticles.Stop ();
					mMesh.enabled = false;
					mBody.isKinematic = true;
					mCollider.isTrigger = true;
					break;
				case eProjectileState.fired:
					{
						Vector3 fwd = mTankParent.mRailgun.transform.forward;
						Vector3 pos = mTankParent.mMuzzle.transform.position;
						int layerMask = 1 << LayerMask.NameToLayer ("Default");
						RaycastHit hit;
						if (Physics.Raycast (pos, fwd, out hit, 100000, layerMask))
						{
							Vector3 vel = Helpers.VelocityToHitPointWithSetApex ((hit.point.y + pos.y) + mTrajectoryApexOffset, pos, hit.point);
							mMesh.enabled = true;
							mCollider.isTrigger = false;
							mBody.velocity = vel;
							NetworkServer.Spawn (gameObject);
						}
						break;
					}
				case eProjectileState.hit:
					GameObject go = Instantiate (mHitParticle, mBody.transform.position, Quaternion.identity);
					go.GetComponent<ParticleSystem> ().Play ();
					NetworkServer.Spawn(go);
					SetState (eProjectileState.idle);
					break;
			}
		}

		protected override void Update ()
		{
			base.Update ();
			switch (mState)
			{
				case eProjectileState.fired:
					mTransform.forward = mBody.velocity.normalized;
					break;
			}
		}

		protected override void OnCollisionEnter (Collision other)
		{
			base.OnCollisionEnter(other);
		}
	}
}