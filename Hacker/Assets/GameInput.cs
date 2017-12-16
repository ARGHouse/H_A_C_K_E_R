﻿namespace HACKER
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	public class GameInput : Helpers.HelperSingleton<GameInput>
	{
		public Camera mCamera { get; private set; }
		public Vector3 mMouseWorldPosition { get; private set; }
		public string mHitName { get; private set; }
		public GameObject mHitObject { get; private set; }
		public bool mLShiftIsDown { get; private set; }
		public bool mLCntrlIsDown { get; private set; }

		/// for raycasting. Probably shouldn't be in this class, but it's easier.
		public GameObject mSelectedObj { get; private set; }

		public static GameInput mGameInput { get; private set; }

		public override void Awake()
		{
			base.Awake();
			mGameInput = this;
			mSelectedObj = null;
			mCamera = Camera.main;
			mMouseWorldPosition = new Vector3(0, 0, 0);
			mHitName = "n/a";
			mHitObject = null;
			mLShiftIsDown = false;
			mLCntrlIsDown = false;
		}

		/// processes our game input.
		void Update()
		{
			CheckInput();
			CheckRaycast();
		}

		void FixedUpdate()
		{
			CheckFixedInput();
		}

		/// for calls that apply physics movement.
		private void CheckFixedInput()
		{
			if (Input.GetButton("Forward"))
				OnForwardIsDown();
			if (Input.GetButton("Backward"))
				OnBackwardIsDown();
			if (Input.GetButton("Left"))
				OnLeftIsDown();
			if (Input.GetButton("Right"))
				OnRightIsDown();
			if (Input.GetButton("RotateRight"))
				OnRotateRightIsDown();
			if (Input.GetButton("RotateLeft"))
				OnRotateLeftIsDown();
			if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
				OnMouseWheelUp();
			if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
				OnMouseWheelDown();
		}

		/// Checks for mouse click/drag events.
		private void CheckInput()
		{
			/////////////////////////////////////////
			/// left click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonUp("LeftClick"))
				OnLeftClickUp();
			else if (Input.GetButtonDown("LeftClick"))
			{
				OnLeftClickDown();
			}

			if (Input.GetButton("LeftClick"))
				OnLeftClickIsDown();

			/////////////////////////////////////////
			/// right click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonDown("RightClick"))
			{
				OnRightClickDown();
			}
			else if (Input.GetButtonUp("RightClick"))
				OnRightClickUp();

			if (Input.GetButton("RightClick"))
				OnRightClickIsDown();

			/////////////////////////////////////////
			/// middle click/////////////////////////
			/////////////////////////////////////////

			if (Input.GetButtonDown("MiddleClick"))
			{
				CheckRaycast();
				OnMiddleClickDown();
			}
			else if (Input.GetButtonUp("MiddleClick"))
				OnMiddleClickUp();

			if (Input.GetButton("MiddleClick"))
				OnMiddleClickIsDown();

			/////////////////////////////////////////
			/// modifiers////////////////////////////
			/////////////////////////////////////////

			mLCntrlIsDown = Input.GetButton("LCntrl");
			mLShiftIsDown = Input.GetButton("LShift");
		}

		/// Typical raycast. we store the hit object for use by other classes
		public void CheckRaycast()
		{
			Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool raycast = Physics.Raycast(ray, out hit, 10000);

			mMouseWorldPosition = hit.point;

			if (raycast)
			{
				mHitName = hit.collider.gameObject.name;
				mHitObject = hit.collider.gameObject;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Event Handlers////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// Event for box drag finished.
		public delegate void LeftClickDownEventHandler(object source, EventArgs args);
		public event LeftClickDownEventHandler LeftClickDown;
		public void OnLeftClickDown()
		{
			if (LeftClickDown != null)
				LeftClickDown(this, EventArgs.Empty);
		}

		/// Event for box drag finished.
		public delegate void LeftClickUpEventHandler(object source, EventArgs args);
		public event LeftClickUpEventHandler LeftClickUp;
		public void OnLeftClickUp()
		{
			if (LeftClickUp != null)
				LeftClickUp(this, EventArgs.Empty);
		}

		/// Event for left click being down.
		public delegate void LeftClickIsDownEventHandler(object source, EventArgs args);
		public event LeftClickIsDownEventHandler LeftClickIsDown;
		public void OnLeftClickIsDown()
		{
			if (LeftClickIsDown != null)
				LeftClickIsDown(this, EventArgs.Empty);
		}

		///RightClick:
		/// Event for box drag finished.
		public delegate void RightClickDownEventHandler(object source, EventArgs args);
		public event LeftClickDownEventHandler RightClickDown;
		public void OnRightClickDown()
		{
			if (RightClickDown != null)
				RightClickDown(this, EventArgs.Empty);
		}

		/// Event for box drag finished.
		public delegate void RightClickUpEventHandler(object source, EventArgs args);
		public event RightClickUpEventHandler RightClickUp;
		public void OnRightClickUp()
		{
			if (RightClickUp != null)
				RightClickUp(this, EventArgs.Empty);
		}

		/// Event for left click being down.
		public delegate void RightClickIsDownEventHandler(object source, EventArgs args);
		public event RightClickIsDownEventHandler RightClickIsDown;
		public void OnRightClickIsDown()
		{
			if (RightClickIsDown != null)
				RightClickIsDown(this, EventArgs.Empty);
		}

		/// Event for MiddleClickDown
		public delegate void MiddleClickDownEventHandler(object source, EventArgs args);
		public event MiddleClickDownEventHandler MiddleClickDown;
		public void OnMiddleClickDown()
		{
			if (MiddleClickDown != null)
				MiddleClickDown(this, EventArgs.Empty);
		}

		/// Event for Middle click up
		public delegate void MiddleClickUpEventHandler(object source, EventArgs args);
		public event MiddleClickUpEventHandler MiddleClickUp;
		public void OnMiddleClickUp()
		{
			if (MiddleClickUp != null)
				MiddleClickUp(this, EventArgs.Empty);
		}

		/// Event for Middle click being down.
		public delegate void MiddleClickIsDownEventHandler(object source, EventArgs args);
		public event MiddleClickIsDownEventHandler MiddleClickIsDown;
		public void OnMiddleClickIsDown()
		{
			if (MiddleClickIsDown != null)
				MiddleClickIsDown(this, EventArgs.Empty);
		}

		///////////////////////////
		//camera movements/////////
		///////////////////////////
		///RightClick:
		public delegate void ForwardIsDownEventHandler(object source, EventArgs args);
		public event ForwardIsDownEventHandler ForwardIsDown;
		public void OnForwardIsDown()
		{
			if (ForwardIsDown != null)
				ForwardIsDown(this, EventArgs.Empty);
		}

		public delegate void BackwardIsDownEventHandler(object source, EventArgs args);
		public event BackwardIsDownEventHandler BackwardIsDown;
		public void OnBackwardIsDown()
		{
			if (BackwardIsDown != null)
				BackwardIsDown(this, EventArgs.Empty);
		}

		public delegate void LeftIsDownEventHandler(object source, EventArgs args);
		public event LeftIsDownEventHandler LeftIsDown;
		public void OnLeftIsDown()
		{
			if (LeftIsDown != null)
				LeftIsDown(this, EventArgs.Empty);
		}

		public delegate void RightIsDownEventHandler(object source, EventArgs args);
		public event RightIsDownEventHandler RightIsDown;
		public void OnRightIsDown()
		{
			if (RightIsDown != null)
				RightIsDown(this, EventArgs.Empty);
		}

		public delegate void RotateRightIsDownEventHandler(object source, EventArgs args);
		public event RightIsDownEventHandler RotateRightIsDown;
		public void OnRotateRightIsDown()
		{
			if (RotateRightIsDown != null)
				RotateRightIsDown(this, EventArgs.Empty);
		}

		public delegate void RotateLeftIsDownEventHandler(object source, EventArgs args);
		public event RotateLeftIsDownEventHandler RotateLeftIsDown;
		public void OnRotateLeftIsDown()
		{
			if (RotateLeftIsDown != null)
				RotateLeftIsDown(this, EventArgs.Empty);
		}

		//OnMouseWheel
		public delegate void MouseWheelUpEventHandler(object source, EventArgs args);
		public event MouseWheelUpEventHandler MouseWheelUp;
		public void OnMouseWheelUp()
		{
			if (MouseWheelUp != null)
				MouseWheelUp(this, EventArgs.Empty);
		}
		public delegate void MouseWheelDownEventHandler(object source, EventArgs args);
		public event MouseWheelDownEventHandler MouseWheelDown;
		public void OnMouseWheelDown()
		{
			if (MouseWheelDown != null)
				MouseWheelDown(this, EventArgs.Empty);
		}
	}
}