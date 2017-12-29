namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	/// sits idle until player is in view.
	public class EnemyAlertIdle : Task
	{
		public enum eEnemyAlertIdleState { alertIdle }

		protected override void Awake ()
		{
			base.Awake ();
			SetState ((int) eEnemyAlertIdleState.alertIdle);
		}

		public override void UpdateTask ()
		{
			base.UpdateTask ();
			float distance = Vector3.Distance (mBrain.gameObject.transform.position, TrojanTank.mTank.gameObject.transform.position);
			if (distance < mBrain.mAlertRadius)
			{
				///TODO: need to raycast to make sure we're actually visible from this object to the player.
				mBrain.AddTask (new Chase (), mPriority);
				mBrain.RemoveTask(this);
			}
		}
	}
}