namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// our game states.
	public enum eGameState { menu, paused, playing };
	public class HackerGameManager : MonoBehaviour
	{
		public eGameState mState { get; private set; }
		public eGameState mPreviousState { get; private set; }
		public float mStateTimer { get; private set; }
		public float mDt { get; private set; }
		public static HackerGameManager mGame { get; private set; }
		void Awake()
		{
			mState = eGameState.playing;
			mPreviousState = mState;
			mStateTimer = 0.0f;
			mDt = 0.0f;
			mGame = this;
		}

		public void SetState(eGameState stateIN)
		{
			mPreviousState = mState;
			mState = stateIN;
			mStateTimer = 0.0f;
			switch (mState)
			{
				case eGameState.menu:
					break;
				case eGameState.playing:
					break;
				case eGameState.paused:
					break;
			}
		}

		void Update() { UpdateState(); }
		private void UpdateState()
		{
			mDt = Time.deltaTime;
			mStateTimer += mDt;
			switch (mState)
			{
				case eGameState.menu:
					break;
				case eGameState.playing:
					break;
				case eGameState.paused:
					break;
			}
		}
	}
}