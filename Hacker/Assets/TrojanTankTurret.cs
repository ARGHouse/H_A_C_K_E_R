namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TrojanTankTurret : MonoBehaviour
	{
		public Transform mTransform { get; private set; }
		public Camera mCam { get; private set; }
		public Vector3 mCamOffset { get; private set; }
		private float currentX = 0.0f;
		private float currentY = 0.0f;
		[SerializeField]
		private float sensitivityX = 4.0f;
		[SerializeField]
		private float sensitivityY = 1.0f;
		private const float Y_ANGLE_MIN = 0.0f;
		private const float Y_ANGLE_MAX = 50.0f;
		public GameInput mGameInput { get; private set; }
		protected virtual void Awake()
		{
			mCam = Camera.main;
			mTransform = transform;
			mCamOffset = mCam.transform.position - mTransform.position;
		}

		protected virtual void Start()
		{
			mGameInput = GameInput.mGameInput;
		}

		protected virtual void Update()
		{
			
		}
	}
}