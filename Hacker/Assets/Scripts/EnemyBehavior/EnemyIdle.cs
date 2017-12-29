namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	/// This class quite literally does nothing.
	/// used as the base task. priority 0, never destroys itself.
	/// if all else fails, the entity will sit there.
	public class EnemyIdle : Task
	{
		public enum eIdleStates { idle }
		protected override void Awake ()
		{
			base.Awake ();
			SetState ((int) eIdleStates.idle);
			mPriority = 0;
		}

		public override void UpdateTask ()
		{
			base.UpdateTask ();
			switch (mState)
			{
				case (int) eIdleStates.idle:
					break;
			}
		}
	}
}