namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
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
						mParticles.Play ();
						/// we disable kinematic a tick before when fire() gets called.
						/// applying force during the same tick goes badly.
						Vector3 fwd = mTankParent.mTurret.mRailgun.transform.forward;
						Vector3 pos = mTankParent.mTurret.mMuzzle.transform.position;
						int layerMask = 1 << LayerMask.NameToLayer ("Default");
						RaycastHit hit;
						if (Physics.Raycast (pos, fwd, out hit, 100000, layerMask))
						{
							float distanceToHit = Vector3.Distance (mTankParent.mTurret.mMuzzle.transform.position, hit.point);
							print (hit.point.x + " " + hit.point.y + " " + hit.point.z + " " + distanceToHit);

							Vector3 vel = Helpers.VelocityToHitPointWithSetApex ((hit.point.y + pos.y) + mTrajectoryApexOffset, pos, hit.point);
							mMesh.enabled = true;
							mCollider.isTrigger = false;
							mBody.AddForce (vel, ForceMode.VelocityChange);
						}
						break;
					}
				case eProjectileState.hit:
					Instantiate (mHitParticle, mBody.transform.position, Quaternion.identity).GetComponent<ParticleSystem> ().Play ();
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
	}
}