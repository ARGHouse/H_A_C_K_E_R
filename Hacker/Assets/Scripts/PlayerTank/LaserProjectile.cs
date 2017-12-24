namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// child of the projectile class. basic laser.
	public class LaserProjectile : Projectile
	{
		protected override void SetState(eProjectileState state)
		{
			base.SetState(state);
			switch (mState)
			{
				case eProjectileState.idle:
					mMesh.enabled = false;
					mBody.isKinematic = true;
					mCollider.isTrigger = true;
					break;
				case eProjectileState.fired:
					{
						/// we disable kinematic a tick before when fire() gets called.
						/// applying force during the same tick goes badly.
						mMesh.enabled = true;
						mCollider.isTrigger = false;
						mBody.AddForce(mBody.transform.forward * mFireForce, ForceMode.Impulse);
						break;
					}
				case eProjectileState.hit:
					Instantiate(Resources.Load("Particles/Spark"), mBody.transform.position, Quaternion.identity);
					SetState(eProjectileState.idle);
					break;
			}
		}
	}
}