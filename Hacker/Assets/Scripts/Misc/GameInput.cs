namespace HACKER
{
	using System.Collections.Generic;
	using System.Collections;
	using System;
	using UnityEngine;

	public class GameInput : Helpers.HelperSingleton<GameInput>
	{
		public Camera mCamera { get; private set; }
		public Vector3 mMouseWorldPosition { get; private set; }

		/// current raycast object selected by mouse.
		public string mHitName { get; private set; }
		public GameObject mHitObject { get; private set; }
		public Vector3 mRayHitPos { get; private set; }

		/// used to determine whether we're trying to rotate the turret via mouse
		public bool mMouseRotateModifier { get; private set; }

		/// platform strings:
		public enum ePlatform { winController, macController, linuxController, MouseAndKey };
		public static GameInput mGameInput { get; private set; }
		private ePlatform mPlatform = ePlatform.winController;
		private string[] mFireKey = new string[] { "joystick button 0", "joystick button 0", "joystick button 0", "mouse 0" };

		/// rotation
		public float mHorizontalRotation { get; private set; }
		public float mVerticalRotation { get; private set; }
		private string[] mHorizontalRotateKey = new string[] { "Joystick_4_Axis", "Joystick_3_Axis", "Joystick_4_Axis", "Horizontal" };
		private string[] mVerticalRotateKeyKey = new string[] { "Joystick_5_Axis", "Joystick_4_Axis", "Joystick_5_Axis", "Vertical" };

		/// throttle
		public float mLeftThrottle { get; private set; }
		public float mRightThrottle { get; private set; }
		private string[] mLeftThrottleForwardKey = new string[] { "Joystick_9_Axis", "Joystick_5_Axis", "Joystick_3_Axis", "LeftForward" };
		private string[] mLeftThrottleReverseKey = new string[] { "joystick button 4", "joystick button 13", "joystick button 4", "LeftBackward" };
		private string[] mRightThrottleForwardKey = new string[] { "Joystick_10_Axis", "Joystick_6_Axis", "Joystick_6_Axis", "RightForward" };
		private string[] mRightThrottleReverseKey = new string[] { "joystick button 5", "joystick button 14", "joystick button 5", "RightBackward" };

		/// for raycasting. Probably shouldn't be in this class, but it's easier.
		public GameObject mSelectedObj { get; private set; }

		/// layer masks:
		public LayerMask mDefaultMask { get; private set; }
		public LayerMask mPlayerMask { get; private set; }

		[SerializeField]
		private float mDeadZone = 0.1f;
		public HackerGameManager mManager { get; private set; }

		public override void Awake ()
		{
			base.Awake ();
			mGameInput = this;
			mSelectedObj = null;
			mCamera = Camera.main;
			mMouseWorldPosition = new Vector3 (0, 0, 0);
			mHitName = "n/a";
			mHitObject = null;
			mRayHitPos = new Vector3 (0, 0, 0);
			mDefaultMask = LayerMask.NameToLayer ("Default");
			mPlayerMask = LayerMask.NameToLayer ("Player");
			mHorizontalRotation = 0.0f;
			mVerticalRotation = 0.0f;
			mLeftThrottle = 0.0f;
			mRightThrottle = 0.0f;
		}

		private ePlatform GetPlatform ()
		{
			if (!mManager.UseController)
				mPlatform = ePlatform.MouseAndKey;
			else
			{
				mPlatform = ePlatform.winController;
				if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
					mPlatform = ePlatform.linuxController;
				if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
					mPlatform = ePlatform.macController;
			}
			return mPlatform;
		}

		void Start ()
		{
			mManager = HackerGameManager.mGame;
			mPlatform = GetPlatform ();
		}

		/// processes our game input.
		void Update ()
		{
			/// separated, as controller uses axes instead of buttons.
			if (mPlatform == ePlatform.MouseAndKey)
				CheckKeyboardInput ();

			/// both controller and keyboard/mouse
			CheckInput ();
			CheckRaycast ();
			CheckGeometryRaycast ();
		}

		void FixedUpdate ()
		{
			/// separated as keyboard will use buttons rather than axes.
			if (mPlatform == ePlatform.MouseAndKey)
				CheckKeyboardFixedInput ();
			else
				CheckControllerFixedInput ();
		}

		/// keys that apply physics movement.
		/// Please only use 'is down' or axes for these.
		/// up/down events may not be detected due to the fixed time step.
		private void CheckKeyboardFixedInput ()
		{
			mLeftThrottle = 0;
			mRightThrottle = 0;
			if (Input.GetButton (mLeftThrottleForwardKey[(int) mPlatform]))
			{
				mLeftThrottle += 1;
				OnLeftThrottleIsDown ();
			}
			if (Input.GetButton (mRightThrottleForwardKey[(int) mPlatform]))
			{
				mRightThrottle += 1;
				OnRightThrottleIsDown ();
			}
			if (Input.GetButton (mLeftThrottleReverseKey[(int) mPlatform]))
			{
				mLeftThrottle -= 1;
				OnLeftThrottleIsDown ();
			}
			if (Input.GetButton (mRightThrottleReverseKey[(int) mPlatform]))
			{
				mRightThrottle -= 1;
				OnRightThrottleIsDown ();
			}
		}

		/// controller buttons that apply physics movement.
		/// Please only use 'is down' or axes for these.
		/// up/down events may not be detected due to the fixed time step.
		private void CheckControllerFixedInput ()
		{
			// reverse are buttons rather than keys.
			mLeftThrottle = 0;
			mLeftThrottle += Input.GetAxisRaw (mLeftThrottleForwardKey[(int) mPlatform]);
			if (Input.GetKey (mLeftThrottleReverseKey[(int) mPlatform]))
				mLeftThrottle -= 1;

			mRightThrottle = 0;
			mRightThrottle += Input.GetAxisRaw (mRightThrottleForwardKey[(int) mPlatform]);
			if (Input.GetKey (mRightThrottleReverseKey[(int) mPlatform]))
				mRightThrottle -= 1;

			/// apply if necessary
			if (mLeftThrottle > mDeadZone || mLeftThrottle < -mDeadZone)
				OnLeftThrottleIsDown ();
			if (mRightThrottle > mDeadZone || mRightThrottle < -mDeadZone)
				OnRightThrottleIsDown ();
		}

		/// updated every frame. used for events. Same for both controllers and keyboard.
		private void CheckInput ()
		{
			mHorizontalRotation = Input.GetAxisRaw (mHorizontalRotateKey[(int) mPlatform]);
			mVerticalRotation = Input.GetAxisRaw (mVerticalRotateKeyKey[(int) mPlatform]);
			if (mHorizontalRotation > mDeadZone)
				OnRotateRightIsDown ();
			if (mHorizontalRotation < -mDeadZone)
				OnRotateLeftIsDown ();
			if (mVerticalRotation > mDeadZone)
				OnRotateUpIsDown ();
			if (mVerticalRotation < -mDeadZone)
				OnRotateDownIsDown ();

			if (Input.GetKeyDown (mFireKey[(int) mPlatform]))
				OnFirePrimaryUp ();
			if (Input.GetKeyDown (mFireKey[(int) mPlatform]))
				OnFirePrimaryDown ();
			if (Input.GetKey (mFireKey[(int) mPlatform]))
				OnFirePrimaryIsDown ();
		}

		/// Checks for mouse click/drag events.
		private void CheckKeyboardInput ()
		{
			/////////////////////////////////////////
			/// right click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonDown ("RightClick"))
			{
				OnRightClickDown ();
			}
			else if (Input.GetButtonUp ("RightClick"))
				OnRightClickUp ();

			mMouseRotateModifier = Input.GetButton ("RightClick");
			if (mMouseRotateModifier)
				OnRightClickIsDown ();
		}

		/// Typical raycast. we store the hit object for use by other classes
		public void CheckRaycast ()
		{
			Ray ray = mCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			bool raycast = Physics.Raycast (ray, out hit, 10000);

			mMouseWorldPosition = hit.point;

			if (raycast)
			{
				mHitName = hit.collider.gameObject.name;
				mHitObject = hit.collider.gameObject;
			}
		}

		public void CheckGeometryRaycast ()
		{
			Ray ray = mCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			int layerMask = 1 << LayerMask.NameToLayer ("Default");
			bool raycast = Physics.Raycast (ray, out hit, 10000, layerMask);
			if (raycast)
				mRayHitPos = hit.point;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Event Handlers////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		///RightClick:
		public delegate void RightClickDownEventHandler (object source, EventArgs args);
		public event RightClickDownEventHandler RightClickDown;
		public void OnRightClickDown ()
		{
			if (RightClickDown != null)
				RightClickDown (this, EventArgs.Empty);
		}

		public delegate void RightClickUpEventHandler (object source, EventArgs args);
		public event RightClickUpEventHandler RightClickUp;
		public void OnRightClickUp ()
		{
			if (RightClickUp != null)
				RightClickUp (this, EventArgs.Empty);
		}

		public delegate void RightClickIsDownEventHandler (object source, EventArgs args);
		public event RightClickIsDownEventHandler RightClickIsDown;
		public void OnRightClickIsDown ()
		{
			if (RightClickIsDown != null)
				RightClickIsDown (this, EventArgs.Empty);
		}

		///////////////////////////
		//camera movements/////////
		///////////////////////////
		public delegate void RotateRightIsDownEventHandler (object source, EventArgs args);
		public event RotateRightIsDownEventHandler RotateRightIsDown;
		public void OnRotateRightIsDown ()
		{
			if (RotateRightIsDown != null)
				RotateRightIsDown (this, EventArgs.Empty);
		}

		public delegate void RotateLeftIsDownEventHandler (object source, EventArgs args);
		public event RotateLeftIsDownEventHandler RotateLeftIsDown;
		public void OnRotateLeftIsDown ()
		{
			if (RotateLeftIsDown != null)
				RotateLeftIsDown (this, EventArgs.Empty);
		}

		public delegate void RotateUpIsDownEventHandler (object source, EventArgs args);
		public event RotateUpIsDownEventHandler RotateUpIsDown;
		public void OnRotateUpIsDown ()
		{
			if (RotateUpIsDown != null)
				RotateUpIsDown (this, EventArgs.Empty);
		}

		public delegate void RotateDownIsDownEventHandler (object source, EventArgs args);
		public event RotateDownIsDownEventHandler RotateDownIsDown;
		public void OnRotateDownIsDown ()
		{
			if (RotateDownIsDown != null)
				RotateDownIsDown (this, EventArgs.Empty);
		}

		/// throttle:
		public delegate void LeftThrottleIsDownEventHandler (object source, EventArgs args);
		public event LeftThrottleIsDownEventHandler LeftThrottleIsDown;
		public void OnLeftThrottleIsDown ()
		{
			if (LeftThrottleIsDown != null)
				LeftThrottleIsDown (this, EventArgs.Empty);
		}

		public delegate void RightThrottleIsDownEventHandler (object source, EventArgs args);
		public event RightThrottleIsDownEventHandler RightThrottleIsDown;
		public void OnRightThrottleIsDown ()
		{
			if (RightThrottleIsDown != null)
				RightThrottleIsDown (this, EventArgs.Empty);
		}

		/// projectiles:
		public delegate void FirePrimaryDownEventHandler (object source, EventArgs args);
		public event FirePrimaryDownEventHandler FirePrimaryDown;
		public void OnFirePrimaryDown ()
		{
			if (FirePrimaryDown != null)
				FirePrimaryDown (this, EventArgs.Empty);
		}
		public delegate void FirePrimaryUpEventHandler (object source, EventArgs args);
		public event FirePrimaryUpEventHandler FirePrimaryUp;
		public void OnFirePrimaryUp ()
		{
			if (FirePrimaryUp != null)
				FirePrimaryUp (this, EventArgs.Empty);
		}
		public delegate void FirePrimaryIsDownEventHandler (object source, EventArgs args);
		public event FirePrimaryIsDownEventHandler FirePrimaryIsDown;
		public void OnFirePrimaryIsDown ()
		{
			if (FirePrimaryIsDown != null)
				FirePrimaryIsDown (this, EventArgs.Empty);
		}
	}
}