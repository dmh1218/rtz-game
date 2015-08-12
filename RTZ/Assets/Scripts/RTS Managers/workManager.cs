﻿using UnityEngine;
using System.Collections.Generic;

namespace RTS {
	public static class workManager  
	{
		public static Rect calculateSelectionBox(Bounds selectionBounds, Rect playingArea) 
		{
			//shorthand for the coordinates of the center of the selection bounds
			float cx = selectionBounds.center.x;
			float cy = selectionBounds.center.y;
			float cz = selectionBounds.center.z;
			
			//shorthand for the coordinates of the extents of the selection bounds
			float ex = selectionBounds.extents.x;
			float ey = selectionBounds.extents.y;
			float ez = selectionBounds.extents.z;
			
			//determine the screen coordinates for the corners of the selection bounds
			List< Vector3 > corners = new List< Vector3 > ();
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz + ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz - ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz + ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz + ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz - ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz + ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz - ez)));
			corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz - ez)));
			
			//determine the bounds on screen for the selection bounds
			Bounds screenBounds = new Bounds (corners [0], Vector3.zero);
			for (int i = 1; i < corners.Count; i++) {
				screenBounds.Encapsulate (corners [i]);
			}
			
			//screen coordinates start in the bottom left corner, rather than the top left corner
			//this correction is needed to make sure the selection box is drawn in the correct place
			float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
			float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
			float selectBoxWidth = 2 * screenBounds.extents.x;
			float selectBoxHeight = 2 * screenBounds.extents.y;
			
			return new Rect (selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
		}
		
		public static GameObject findHitObject(Vector3 origin)
		{
			Ray ray = Camera.main.ScreenPointToRay (origin);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				return hit.collider.gameObject;
			} else {
				return null;
			}
		}
		
		public static Vector3 findHitPoint(Vector3 origin)
		{
			Ray ray = Camera.main.ScreenPointToRay (origin);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				return hit.point;
			} else {
				return resourceManager.InvalidPosition;
			}
		}

		public static resourceType getResourceType(string type)
		{
			switch (type) {
			case "Money":
				return resourceType.Money;
//			case "Power":
//				return resourceType.Power;
			case "Ore":
				return resourceType.Ore;
			case "Food":
				return resourceType.Food;
			default:
				return resourceType.Unknown;
			}
		}

		public static bool objectIsGround(GameObject obj)
		{
			return obj.name == "Ground" || obj.name == "Ground(Clone)";
		}

		public static List<WorldObject> findNearbyObjects(Vector3 position, float range)
		{
			Collider[] hitColliders = Physics.OverlapSphere (position, range);
			HashSet<int> nearbyObjectIds = new HashSet<int> ();
			List<WorldObject> nearbyObjects = new List<WorldObject> ();

			for (int i = 0; i < hitColliders.Length; i++) {
				Transform parent = hitColliders [i].transform.parent;
				if (parent) {
					WorldObject parentObject = parent.GetComponent<WorldObject> ();
					if (parentObject && !nearbyObjectIds.Contains (parentObject.objectId)) {
						nearbyObjectIds.Add (parentObject.objectId);
						nearbyObjects.Add (parentObject);
					}
				}
			}
			return nearbyObjects;
		}

		public static WorldObject findNearestWorldObject(List<WorldObject> objects, Vector3 position)
		{
			if (objects == null || objects.Count == 0) {
				return null;
			}

			WorldObject nearestObject = objects [0];
			float distanceToNearestObject = Vector3.Distance (position, nearestObject.transform.position);

			for (int i = 1; i < objects.Count; i++) {
				float distanceToObject = Vector3.Distance (position, objects [i].transform.position);
				if (distanceToObject < distanceToNearestObject) {
					distanceToNearestObject = distanceToObject;
					nearestObject = objects [i];
				}
			}
			return nearestObject;
		}
	}
}
