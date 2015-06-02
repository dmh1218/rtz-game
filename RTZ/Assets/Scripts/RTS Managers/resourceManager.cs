using UnityEngine;
using System.Collections.Generic;

namespace RTS {
	public static class resourceManager 
	{
		//camera control constants
		public static float scrollSpeed { get { return 25; } }
		public static float rotateSpeed { get { return 100; } }
		public static float rotateAmount { get { return 10; } }
		public static int scrollwidth { get { return 15; } }
		public static float minCameraHeight { get { return 10; } }
		public static float MaxCameraHeight { get { return 40; } }
		public static float zoomSpeed { get { return 1; } }
		public static float zoomRotation { get { return 1; } }

		//outside map locations and bounds
		private static Vector3 invalidPosition = new Vector3 (-99999, -99999, -99999);
		public static Vector3 InvalidPosition { get { return invalidPosition; } }
		private static Bounds invalidBounds = new Bounds (new Vector3 (-99999, -99999, -99999), new Vector3 (0, 0, 0));
		public static Bounds InvalidBounds { get { return invalidBounds; } }

		//selection box stuff
		private static GUISkin selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

		public static void storeSelectBoxItems(GUISkin skin, Texture2D healthy, Texture2D damaged, Texture2D critical)
		{
			selectBoxSkin = skin;
			healthyTexture = healthy;
			damagedTexture = damaged;
			criticalTexture = critical;
		}

		//the speed at which units are trained from buildings - customize later
		public static int buildSpeed { get { return 2; } }

		//create and set gameObjectList
		private static GameObjectList gameObjectList;
		public static void setGameObjectList(GameObjectList objectList)
		{
			gameObjectList = objectList;
		}

		//get functions from gameObjectList
		public static GameObject getBuilding(string name)
		{
			return gameObjectList.getBuilding (name);
		}

		public static GameObject getUnit(string name)
		{
			return gameObjectList.getUnit (name);
		}

		public static GameObject getWorldObject(string name)
		{
			return gameObjectList.getWorldObject (name);
		}

		public static GameObject getPlayerObject()
		{
			return gameObjectList.getPlayerObject ();
		}

		public static Texture2D getBuildImage(string name)
		{
			return gameObjectList.getBuildImage (name);
		}

		//health bar textures
		private static Texture2D healthyTexture, damagedTexture, criticalTexture;
		public static Texture2D HealthyTexture { get { return healthyTexture; } }
		public static Texture2D DamagedTexture { get { return damagedTexture; } }
		public static Texture2D CriticalTexture { get { return criticalTexture; } }

		//resource health bar textures
		private static Dictionary<resourceType, Texture2D> resourceHealthBarTextures;

		public static Texture2D getResourceHealthBar(resourceType resType)
		{
			if (resourceHealthBarTextures != null && resourceHealthBarTextures.ContainsKey (resType)) {
				return resourceHealthBarTextures [resType];
			}
			return null;
		}

		public static void setResourceHealthBarTextures (Dictionary<resourceType, Texture2D> images)
		{
			resourceHealthBarTextures = images;
		}

		//pause menu stuff
		private static float ButtonHeight = 40;
		private static float HeaderHeight = 32, HeaderWidth = 256;
		private static float TextHeight = 25, Padding = 10;
		public static bool menuOpen { get; set; }
		public static float pauseMenuHeight { get { return HeaderHeight + 2 * ButtonHeight + 4 * Padding; } }
		public static float menuWidth { get { return HeaderWidth + 2 * Padding; } }
		public static float buttonHeight { get { return ButtonHeight; } }
		public static float buttonWidth { get { return (menuWidth - 3 * Padding) / 2; } }
		public static float headerHeight { get { return HeaderHeight; } }
		public static float headerWidth { get { return HeaderWidth; } }
		public static float textHeight { get { return TextHeight; } }
		public static float padding { get { return Padding; } }


	}
}
