namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class LaserHit : MonoBehaviour
	{
		[SerializeField]
		private float mLifeTime = 3.0f;
		private float mCurrentTimeAlive = 0.0f;
		
		void Update()
		{
			mCurrentTimeAlive += Time.deltaTime;
			if(mCurrentTimeAlive > mLifeTime)
				Destroy(gameObject, 0.0f);
		}
	}
}