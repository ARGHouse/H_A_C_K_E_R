﻿namespace HACKER
{
	using System.Collections.Generic;
	using System.IO;
	using System;
	using UnityEngine;

	public delegate void HelperDelegate ();

	/// just a pair class implementation, very little functionality, sorry :P
	public class Pair<T, U>
	{
		public Pair () { }

		public Pair (T first, U second)
		{
			this.First = first;
			this.Second = second;
		}

		public T First { get; set; }
		public U Second { get; set; }
	};

	//used for vec3 indexing. Typecasting gets annoying if all it's used for is an index.
	//we don't need any of the other vector3 functionality.
	[Serializable]
	public class IntVec3
	{
		public int x;
		public int y;
		public int z;
		public IntVec3 () { }
		public IntVec3 (int xIN, int yIN, int zIN) { x = xIN; y = yIN; z = zIN; }
		public static IntVec3 operator + (IntVec3 a, IntVec3 b) { return new IntVec3 (a.x + b.x, a.y + b.y, a.z + b.z); }
		public static bool operator == (IntVec3 a, IntVec3 b) { return (a.x == b.x && a.y == b.y && a.z == b.z); }
		public static bool operator != (IntVec3 a, IntVec3 b) { return (a.x != b.x || a.y != b.y || a.z != b.z); }
		public override bool Equals (object obj)
		{
			IntVec3 r = obj as IntVec3;
			if (r != null) return r == this;
			return false;
		}

		public override int GetHashCode ()
		{
			return x ^ y ^ z;
		}

		public static float Distance (IntVec3 a, IntVec3 b)
		{
			float deltaX = a.x - b.x;
			float deltaY = a.y - b.y;
			float deltaZ = a.z - b.z;

			return (float) Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
		}
		//distance between intvec3 and vector3
		public static float Distance (IntVec3 a, Vector3 b)
		{
			float deltaX = a.x - b.x;
			float deltaY = a.y - b.y;
			float deltaZ = a.z - b.z;

			return (float) Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
		}
	}

	//used for vec3 indexing. Typecasting gets annoying if all it's used for is an index.
	//we don't need any of the other vector3 functionality.
	public class IntVec2
	{
		public int x;
		public int y;
		public int z;
		public IntVec2 () { }
		public IntVec2 (int xIN, int yIN) { x = xIN; y = yIN; }
		public static IntVec2 operator + (IntVec2 a, IntVec2 b) { return new IntVec2 (a.x + b.x, a.y + b.y); }
		public static bool operator == (IntVec2 a, IntVec2 b) { return (a.x == b.x && a.y == b.y); }
		public static bool operator != (IntVec2 a, IntVec2 b) { return (a.x != b.x || a.y != b.y); }
		public static float Distance (IntVec2 a, IntVec2 b)
		{
			float deltaX = a.x - b.x;
			float deltaY = a.y - b.y;

			return (float) Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY);
		}
		//distance between intvec2 and vector2
		public static float Distance (IntVec2 a, Vector2 b)
		{
			float deltaX = a.x - b.x;
			float deltaY = a.y - b.y;

			return (float) Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY);
		}
		public override bool Equals (object obj)
		{
			IntVec2 r = obj as IntVec2;
			if (r != null) return r == this;
			return false;
		}

		public override int GetHashCode ()
		{
			return x ^ y;
		}
	}

	public class Helpers : MonoBehaviour
	{
		/// just some easier zero values:
		public static Vector2 v2Zero = new Vector2 (0, 0);
		public static Vector3 v3Zero = new Vector3 (0, 0, 0);
		public static IntVec2 iv2Zero = new IntVec2 (0, 0);
		public static IntVec3 iv3Zero = new IntVec3 (0, 0, 0);

		//TO NOTE: not thread safe. use only for object you'll only have one of (like game input handlers.)
		//please, let it wake itself up and don't try to access object in awake() :P
		//To ensure only a single instance of this class.
		public class HelperSingleton<T> : MonoBehaviour where T : Component
		{
			private static T instance;
			public static T Instance
			{
				get
				{
					if (instance == null)
					{
						instance = FindObjectOfType<T> ();
						if (instance == null)
						{
							GameObject obj = new GameObject ();
							obj.name = typeof (T).Name;
							instance = obj.AddComponent<T> ();
						}
					}
					return instance;
				}
			}

			public virtual void Awake ()
			{
				if (instance == null)
				{
					instance = this as T;
					if (transform == transform.root)
						DontDestroyOnLoad (this.gameObject);
				}
				else
					Destroy (gameObject);
			}
		}

		/// returns RaycastHit2D. just pass -1 if you're don't care what mask
		public static RaycastHit2D Get2DRayHit (LayerMask mask)
		{
			if (mask != -1)
				return Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, -Mathf.Infinity, mask);
			return Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		}

		public static RaycastHit2D Get2DRayHitFrom (LayerMask mask, Vector3 cursorPos)
		{
			if (mask != -1)
				return Physics2D.Raycast (cursorPos, Vector2.zero, -Mathf.Infinity, mask);
			return Physics2D.Raycast (cursorPos, Vector2.zero);
		}

		public static Vector3 GetWorldMousePoint ()
		{
			return Camera.main.ScreenToWorldPoint (Input.mousePosition);;
		}

		public static GameObject LoadGameObjFromBundle (string bundleName, string objName)
		{
			string platform = "/Windows";
			if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
				platform = "/Linux";
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
				platform = "/Mac";

			string path = Path.Combine (Application.streamingAssetsPath + platform, bundleName);

			var myLoadedAssetBundle = AssetBundle.LoadFromFile (path);

			if (myLoadedAssetBundle != null)
			{
				GameObject go = myLoadedAssetBundle.LoadAsset<GameObject> (objName);
				myLoadedAssetBundle.Unload (false);
				return go;
			}
			else
				throw new ArgumentNullException ("asset bundle for " + bundleName + " could not be loaded or does not exist.");
		}

		public static Vector3 FlattenDir (Vector3 vecIN)
		{
			return new Vector3 (vecIN.x, 0, vecIN.z);
		}

		//////////////////////////////////////////////////////////////////////////
		/// Physics //////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////

		/// returns falling rate: 1/2 Gravity squared:
		public static float FallDistanceOverTime (float timeIN)
		{
			return 0.5f * Physics.gravity.y * (timeIN * timeIN);
		}

		/// returns negative if it's not possible.
		public static float TrajectoryTravelTimeY (float zVelocity, float startY, float endY)
		{
			float timeOUT = 0.0f;
			float maxHeightAboveStart = (zVelocity * zVelocity) / (2 * Physics.gravity.y);
			if (maxHeightAboveStart < endY)
				return -1;
			float fallHeight = maxHeightAboveStart - endY;
			timeOUT = (zVelocity / Physics.gravity.y) + Mathf.Sqrt ((maxHeightAboveStart + fallHeight) / Physics.gravity.y);
			return timeOUT;
		}

		/// hit point of the trajectory at height y. returns 0,0,0 if not possible.
		public static Vector3 HitPointOfTrajectoryAtY (Vector3 velocity, Vector3 start, float knownY)
		{
			Vector3 endPoint = new Vector3(0,0,0);
			float time = TrajectoryTravelTimeY (velocity.y, start.y, knownY);
			if (time < 0)
				return new Vector3 (0, 0, 0);

			endPoint.x = start.x + (velocity.x * time) + FallDistanceOverTime (time);
			endPoint.y = knownY;
			endPoint.z = start.z + (velocity.z * time) + FallDistanceOverTime (time);
			return endPoint;
		}

		//////////////////////////////////////////////////////////////////////////
		/// finds the position an object with known velocity and start position will be at a final point
		/// on the X axis
		public static Vector3 HitPointOfTrajectoryAtX (Vector3 velocity, Vector3 start, float knownX )
		{
			Vector3 returnPoint = new Vector3(0,0,0);

			if ((knownX >= 0 && velocity.x <= 0) ||
				(knownX <= 0 && velocity.x >= 0))
				return returnPoint;

			Vector3 flatPos = new Vector3 (start.x, 0, start.z);
			Vector3 flatVel = new Vector3 (velocity.x, 0, velocity.z);
			Vector3 flatNorm = flatVel;

			flatNorm.Normalize ();

			float xDist = Mathf.Abs (flatPos.x - knownX);
			float rate = xDist / Mathf.Abs (velocity.x);

			returnPoint.x = knownX;
			returnPoint.z = (velocity.z * rate) + flatPos.z;

			float distance = Vector3.Distance (returnPoint, flatPos);

			float time = distance / flatVel.magnitude;
			returnPoint.y = (start.y + (velocity.y * time) + FallDistanceOverTime (time));
			return returnPoint;
		}

		public static Vector3 HitPointOfTrajectoryAtz (Vector3 velocity, Vector3 start, float knownZ)
		{
			Vector3 returnPoint = new Vector3(0,0,0);
			if ((knownZ >= 0 && velocity.x <= 0) ||
				(knownZ <= 0 && velocity.x >= 0))
				return returnPoint;

			Vector3 flatPos = new Vector3 (start.x, 0, start.z);
			Vector3 flatVel = new Vector3 (velocity.x, 0, velocity.z);
			Vector3 flatNorm = flatVel;

			flatNorm.Normalize ();

			float zDist = Mathf.Abs (flatPos.z - knownZ);
			float rate = zDist / Mathf.Abs (velocity.z);

			returnPoint.z = knownZ;
			returnPoint.x = (velocity.x * rate) + flatPos.x;

			float distance = Vector3.Distance (returnPoint, flatPos);

			float time = distance / flatVel.magnitude;
			returnPoint.y = (start.y + (velocity.y * time) + FallDistanceOverTime (time));
			return returnPoint;
		}

		//////////////////////////////////////////////////////////////////////////
		/// \brief Calculates the needed velocity, given height requirement and start/end positions:
		public static Vector3 VelocityToHitPointWithSetApex (float apex, Vector3 startPos, Vector3 endPos)
		{
			Vector3 velocityOUT = new Vector3(0,0,0);
			if (apex < 0 || apex < endPos.y)
				return new Vector3 (0, 0, 0);

			float time = 0.0f;
			float gravity = -Physics.gravity.y;

			/// upward velocity needed to get us to our height:
			velocityOUT.y = Mathf.Sqrt (2.0f * gravity * (apex - startPos.y));

			//total flight time://
			time = (velocityOUT.y / gravity) + Mathf.Sqrt (2*(apex- endPos.y) / gravity);

			float xVelocity = (endPos.x - startPos.x) / time;
			velocityOUT.x = (Mathf.Abs (xVelocity) > float.MinValue ? xVelocity : 0);
			float zVelocity = (endPos.z - startPos.z) / time;
			velocityOUT.z = (Mathf.Abs (zVelocity) > float.MinValue ? zVelocity : 0);

			return velocityOUT;
		}
	}
}