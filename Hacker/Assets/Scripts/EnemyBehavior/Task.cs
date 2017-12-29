namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class Task
	{
		public int mState { get; protected set; }
		public float mStateTimer { get; protected set; }
		public float mDT { get; protected set; }
		public int mPriority { get; protected set; }
		public Brain mBrain { get; protected set; }
		protected virtual void Awake ()
		{
			mPriority = 0;
			mDT = 0.0f;
			mStateTimer = 0.0f;
			mState = 0;
		}

		public virtual void Init (int priority, Brain brain)
		{
			mPriority = priority;
			mBrain = brain;
		}

		protected virtual void SetState (int stateIN)
		{
			mState = stateIN;
		}

		public virtual void UpdateTask ()
		{
			mDT = Time.deltaTime;
			mStateTimer += mDT;
		}
	}
}