using UnityEngine;
using System.Collections;

namespace RTS {
	public static class resourceManager 
	{
		public static float scrollSpeed { get { return 25; } }
		public static float rotateSpeed { get { return 100; } }
		public static float rotateAmount { get { return 10; } }
		public static int scrollwidth { get { return 15; } }
		public static float minCameraHeight { get { return 10; } }
		public static float MaxCameraHeight { get { return 40; } }
		public static float zoomSpeed { get { return 1; } }
		public static float zoomRotation { get { return 1; } }

		private static Vector3 invalidPosition = new Vector3 (-99999, -99999, -99999);
		public static Vector3 InvalidPosition { get { return invalidPosition; } }

		private static Bounds invalidBounds = new Bounds (new Vector3 (-99999, -99999, -99999), new Vector3 (0, 0, 0));
		public static Bounds InvalidBounds { get { return invalidBounds; } }

		private static GUISkin selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

		public static void storeSelectBoxItems(GUISkin skin)
		{
			selectBoxSkin = skin;
		}


	}
}
