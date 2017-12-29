namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class Brain : MonoBehaviour
	{
		public enum eBrainStates { idle, thinking }
		private eBrainStates mState = eBrainStates.idle;
		private float mStateTimer = 0.0f;
		public int mMaxTasks = 10; ///max number of tasks before we just bail on the lower priority
		private List<Task> mTasks = new List<Task> ();
		private Task mCurrentTask = null;

		/// how far this entity can see:
		[SerializeField]
		public float mAlertRadius = 10.0f;

		private void Awake ()
		{
			mState = eBrainStates.thinking;
			mTasks.Add (new EnemyIdle ());
		}

		/// adds a new task for the brain.
		public void AddTask (Task task, int priority)
		{
			mTasks.Add (new Task ());
			mTasks[mTasks.Count - 1].Init (priority, this);
		}

		void Update () { UpdateState (); }
		private void UpdateState ()
		{
			mStateTimer += Time.deltaTime;
			switch (mState)
			{
				case eBrainStates.thinking:
					UpdateTasks ();
					break;
				default:
					break;
			}
		}

		private void UpdateTasks ()
		{
			mCurrentTask = FindHighestTask ();
			mCurrentTask.UpdateTask ();
		}

		/// finds our highest priority task.
		private Task FindHighestTask ()
		{
			int highest = 0;
			for (int i = 0; i < mTasks.Count; i++)
			{
				if (mTasks[i].mPriority > highest)
					highest = mTasks[i].mPriority;
			}
			return mTasks[highest];
		}

		/// removes a task
		public void RemoveTask(Task task)
		{
			mTasks.Remove(task);
		}
	}
}