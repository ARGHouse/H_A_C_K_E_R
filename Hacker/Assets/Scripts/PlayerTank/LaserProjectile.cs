namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	/// child of the projectile class. basic laser.
	public class LaserProjectile : Projectile
	{
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
						mMesh.enabled = true;
						mCollider.isTrigger = false;
						mBody.AddForce (mBody.transform.forward * mFireForce, ForceMode.Impulse);
						break;
					}
				case eProjectileState.hit:
					Instantiate (mHitParticle, mBody.transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
					SetState (eProjectileState.idle);
					break;
			}
		}
	}
}