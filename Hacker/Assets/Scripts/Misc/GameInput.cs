﻿namespace HACKER
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
		public bool mLShiftIsDown { get; private set; }
		public bool mLCntrlIsDown { get; private set; }
		public bool mRClickIsDown { get; private set; }
		public bool mLClickIsDown { get; private set; }
		public float mMouseHorizontal { get; private set; }
		public float mMouseVertical { get; private set; }
		public float mLeftThrottle { get; private set; }
		public float mRightThrottle { get; private set; }

		/// for raycasting. Probably shouldn't be in this class, but it's easier.
		public GameObject mSelectedObj { get; private set; }

		public static GameInput mGameInput { get; private set; }

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
			mLShiftIsDown = false;
			mLCntrlIsDown = false;
			mDefaultMask = LayerMask.NameToLayer ("Default");
			mPlayerMask = LayerMask.NameToLayer ("Player");
			mMouseHorizontal = 0.0f;
			mMouseVertical = 0.0f;
			mLeftThrottle = 0.0f;
			mRightThrottle = 0.0f;
		}

		void Start ()
		{
			mManager = HackerGameManager.mGame;
		}

		/// processes our game input.
		void Update ()
		{
			if (mManager.UseController)
				CheckControllerInput ();
			else
				CheckKeyboardInput ();

			CheckRaycast ();
			CheckGeometryRaycast ();
		}

		void FixedUpdate ()
		{
			if (!mManager.UseController)
				CheckKeyboardFixedInput ();
			else
				CheckControllerFixedInput ();
		}

		/// keys that apply physics movement.
		/// Please only use 'is down' or axes for these.
		/// up/down events may not be detected due to the fixed time step.
		private void CheckKeyboardFixedInput ()
		{
			if (Input.GetButton ("Forward"))
				OnForwardIsDown ();
			if (Input.GetButton ("Backward"))
				OnBackwardIsDown ();
			if (Input.GetButton ("Left"))
				OnLeftIsDown ();
			if (Input.GetButton ("Right"))
				OnRightIsDown ();
			if (Input.GetButton ("RotateRight"))
				OnRotateRightIsDown ();
			if (Input.GetButton ("RotateLeft"))
				OnRotateLeftIsDown ();
			if (Input.GetAxis ("Mouse ScrollWheel") > 0.0f)
				OnMouseWheelUp ();
			if (Input.GetAxis ("Mouse ScrollWheel") < 0.0f)
				OnMouseWheelDown ();
			if (Input.GetAxis ("Mouse ScrollWheel") < 0.0f)
				OnMouseWheelDown ();
			if (Input.GetAxis ("Mouse ScrollWheel") < 0.0f)
				OnMouseWheelDown ();
		}

		/// controller buttons that apply physics movement.
		/// Please only use 'is down' or axes for these.
		/// up/down events may not be detected due to the fixed time step.
		private void CheckControllerFixedInput ()
		{
			if (Input.GetAxisRaw ("WC_YAxis") < -mDeadZone)
				OnForwardIsDown ();
			if (Input.GetAxisRaw ("WC_YAxis") > mDeadZone)
				OnBackwardIsDown ();
			if (Input.GetAxisRaw ("WC_XAxis") < -mDeadZone)
				OnLeftIsDown ();
			if (Input.GetAxisRaw ("WC_XAxis") > mDeadZone)
				OnRightIsDown ();
			if (Input.GetAxisRaw ("WC_RotateX") > mDeadZone)
				OnRotateRightIsDown ();
			if (Input.GetAxisRaw ("WC_RotateX") < -mDeadZone)
				OnRotateLeftIsDown ();
			if (Input.GetAxisRaw ("WC_RotateY") > mDeadZone)
				OnRotateUpIsDown ();
			if (Input.GetAxisRaw ("WC_RotateY") < -mDeadZone)
				OnRotateDownIsDown ();
			
			mLeftThrottle = Input.GetAxisRaw ("WC_LeftThrottle");
			if (mLeftThrottle > mDeadZone)
				OnLeftThrottleIsDown ();
			mRightThrottle = Input.GetAxisRaw ("WC_RightThrottle");
			if (mRightThrottle> mDeadZone)
				OnRightThrottleIsDown ();

			if(Input.GetButton("WC_ReverseLeftThrottle"))
			{
				mLeftThrottle =  -1;
				OnLeftThrottleIsDown ();
			}
			if(Input.GetButton("WC_ReverseRightThrottle"))
			{
				mRightThrottle =  -1;
				OnRightThrottleIsDown ();
			}
		}

		private void CheckControllerInput ()
		{

		}

		/// Checks for mouse click/drag events.
		private void CheckKeyboardInput ()
		{
			/////////////////////////////////////////
			/// left click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonUp ("LeftClick"))
				OnLeftClickUp ();
			else if (Input.GetButtonDown ("LeftClick"))
			{
				OnLeftClickDown ();
			}

			mLClickIsDown = Input.GetButton ("LeftClick");
			if (mLClickIsDown)
				OnLeftClickIsDown ();

			/////////////////////////////////////////
			/// right click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonDown ("RightClick"))
			{
				OnRightClickDown ();
			}
			else if (Input.GetButtonUp ("RightClick"))
				OnRightClickUp ();

			mRClickIsDown = Input.GetButton ("RightClick");
			if (mRClickIsDown)
				OnRightClickIsDown ();

			/////////////////////////////////////////
			/// middle click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonDown ("MiddleClick"))
			{
				CheckRaycast ();
				OnMiddleClickDown ();
			}
			else if (Input.GetButtonUp ("MiddleClick"))
				OnMiddleClickUp ();

			if (Input.GetButton ("MiddleClick"))
				OnMiddleClickIsDown ();

			/////////////////////////////////////////
			/// modifiers////////////////////////////
			/////////////////////////////////////////

			mLCntrlIsDown = Input.GetButton ("LCntrl");
			mLShiftIsDown = Input.GetButton ("LShift");

			/////////////////////////////////////////
			/// axes ////////////////////////////
			/////////////////////////////////////////
			mMouseHorizontal = Input.GetAxisRaw ("Horizontal");
			mMouseVertical = Input.GetAxisRaw ("Vertical");
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

		/// Event for box drag finished.
		public delegate void LeftClickDownEventHandler (object source, EventArgs args);
		public event LeftClickDownEventHandler LeftClickDown;
		public void OnLeftClickDown ()
		{
			if (LeftClickDown != null)
				LeftClickDown (this, EventArgs.Empty);
		}

		/// Event for box drag finished.
		public delegate void LeftClickUpEventHandler (object source, EventArgs args);
		public event LeftClickUpEventHandler LeftClickUp;
		public void OnLeftClickUp ()
		{
			if (LeftClickUp != null)
				LeftClickUp (this, EventArgs.Empty);
		}

		/// Event for left click being down.
		public delegate void LeftClickIsDownEventHandler (object source, EventArgs args);
		public event LeftClickIsDownEventHandler LeftClickIsDown;
		public void OnLeftClickIsDown ()
		{
			if (LeftClickIsDown != null)
				LeftClickIsDown (this, EventArgs.Empty);
		}

		///RightClick:
		/// Event for box drag finished.
		public delegate void RightClickDownEventHandler (object source, EventArgs args);
		public event LeftClickDownEventHandler RightClickDown;
		public void OnRightClickDown ()
		{
			if (RightClickDown != null)
				RightClickDown (this, EventArgs.Empty);
		}

		/// Event for box drag finished.
		public delegate void RightClickUpEventHandler (object source, EventArgs args);
		public event RightClickUpEventHandler RightClickUp;
		public void OnRightClickUp ()
		{
			if (RightClickUp != null)
				RightClickUp (this, EventArgs.Empty);
		}

		/// Event for left click being down.
		public delegate void RightClickIsDownEventHandler (object source, EventArgs args);
		public event RightClickIsDownEventHandler RightClickIsDown;
		public void OnRightClickIsDown ()
		{
			if (RightClickIsDown != null)
				RightClickIsDown (this, EventArgs.Empty);
		}

		/// Event for MiddleClickDown
		public delegate void MiddleClickDownEventHandler (object source, EventArgs args);
		public event MiddleClickDownEventHandler MiddleClickDown;
		public void OnMiddleClickDown ()
		{
			if (MiddleClickDown != null)
				MiddleClickDown (this, EventArgs.Empty);
		}

		/// Event for Middle click up
		public delegate void MiddleClickUpEventHandler (object source, EventArgs args);
		public event MiddleClickUpEventHandler MiddleClickUp;
		public void OnMiddleClickUp ()
		{
			if (MiddleClickUp != null)
				MiddleClickUp (this, EventArgs.Empty);
		}

		/// Event for Middle click being down.
		public delegate void MiddleClickIsDownEventHandler (object source, EventArgs args);
		public event MiddleClickIsDownEventHandler MiddleClickIsDown;
		public void OnMiddleClickIsDown ()
		{
			if (MiddleClickIsDown != null)
				MiddleClickIsDown (this, EventArgs.Empty);
		}

		///////////////////////////
		//camera movements/////////
		///////////////////////////
		///RightClick:
		public delegate void ForwardIsDownEventHandler (object source, EventArgs args);
		public event ForwardIsDownEventHandler ForwardIsDown;
		public void OnForwardIsDown ()
		{
			if (ForwardIsDown != null)
				ForwardIsDown (this, EventArgs.Empty);
		}

		public delegate void BackwardIsDownEventHandler (object source, EventArgs args);
		public event BackwardIsDownEventHandler BackwardIsDown;
		public void OnBackwardIsDown ()
		{
			if (BackwardIsDown != null)
				BackwardIsDown (this, EventArgs.Empty);
		}

		public delegate void LeftIsDownEventHandler (object source, EventArgs args);
		public event LeftIsDownEventHandler LeftIsDown;
		public void OnLeftIsDown ()
		{
			if (LeftIsDown != null)
				LeftIsDown (this, EventArgs.Empty);
		}

		public delegate void RightIsDownEventHandler (object source, EventArgs args);
		public event RightIsDownEventHandler RightIsDown;
		public void OnRightIsDown ()
		{
			if (RightIsDown != null)
				RightIsDown (this, EventArgs.Empty);
		}

		public delegate void RotateRightIsDownEventHandler (object source, EventArgs args);
		public event RightIsDownEventHandler RotateRightIsDown;
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

		//OnMouseWheel
		public delegate void MouseWheelUpEventHandler (object source, EventArgs args);
		public event MouseWheelUpEventHandler MouseWheelUp;
		public void OnMouseWheelUp ()
		{
			if (MouseWheelUp != null)
				MouseWheelUp (this, EventArgs.Empty);
		}
		public delegate void MouseWheelDownEventHandler (object source, EventArgs args);
		public event MouseWheelDownEventHandler MouseWheelDown;
		public void OnMouseWheelDown ()
		{
			if (MouseWheelDown != null)
				MouseWheelDown (this, EventArgs.Empty);
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
	}
}