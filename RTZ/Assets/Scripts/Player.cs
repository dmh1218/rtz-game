using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour 
{
	//public variables
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject selectedObject { get; set; }
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;
	public Material notAllowedMaterial, allowedMaterial;

	//private variables
	private Dictionary<resourceType, int> resources, resourceLimits;
	private Building tempBuilding;
	private Unit tempCreator;
	private bool findingPlacement = false;

	void Start()
	{
		hud = GetComponentInChildren<HUD> ();
		addStartResourceLimits ();
		addStartResources ();
	}

	void Awake()
	{
		resources = initResourceList ();
		resourceLimits = initResourceList ();
	}

	void Update()
	{
		if (human) {
			hud.setResourceValues (resources, resourceLimits);
		}

		if (findingPlacement) {
			tempBuilding.calculateBounds ();
			if (canPlaceBuilding ()) {
				tempBuilding.setTransparentMaterial (allowedMaterial, false);
			} else {
				tempBuilding.setTransparentMaterial (notAllowedMaterial, false);
			}
		}
	}

	private Dictionary< resourceType, int > initResourceList()
	{
		Dictionary< resourceType, int > list = new Dictionary< resourceType, int > ();
		list.Add (resourceType.Money, 0);
		list.Add (resourceType.Power, 0);
		return list;
	}

	private void addStartResourceLimits()
	{
		incrementResourceLimit (resourceType.Money, startMoneyLimit);
		incrementResourceLimit (resourceType.Power, startPowerLimit);
	}

	private void addStartResources()
	{
		addResource (resourceType.Money, startMoney);
		addResource (resourceType.Power, startPower);
	}

	public void addResource (resourceType type, int amount)
	{
		resources [type] += amount;
	}

	public void incrementResourceLimit (resourceType type, int amount)
	{
		resourceLimits [type] += amount;
	}

	public void addUnit (string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator)
	{
		Debug.Log ("add" + unitName + " to player");
		Units units = GetComponentInChildren<Units> ();
		GameObject newUnit = (GameObject)Instantiate (resourceManager.getUnit (unitName), spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
		Unit unitObject = newUnit.GetComponent<Unit> ();
		if (unitObject && spawnPoint != rallyPoint) {
			unitObject.startMove (rallyPoint);
		}
		if (unitObject) {
			unitObject.setBuilding (creator);
			if (spawnPoint != rallyPoint) {
				unitObject.startMove (rallyPoint);
			}
		}
	}

	public void createBuilding(string buildingName, Vector3 buildPoint, Unit creator, Rect playingArea)
	{
		//maybe create an original materials array to reset to
		GameObject newBuilding = (GameObject)Instantiate (resourceManager.getBuilding (buildingName), buildPoint, new Quaternion ());
		tempBuilding = newBuilding.GetComponent<Building> ();
		if (tempBuilding) {
			tempCreator = creator;
			findingPlacement = true;
			tempBuilding.setTransparentMaterial (notAllowedMaterial, true);
			tempBuilding.setColliders (false);
			tempBuilding.setPlayingArea (playingArea);
		} else {
			Destroy (newBuilding);
		}
	}

	public bool isFindingBuildingLocation()
	{
		return findingPlacement;
	}

	public void findBuildingLocation()
	{
		Vector3 newLocation = workManager.findHitPoint (Input.mousePosition);
		newLocation.y = 0;
		tempBuilding.transform.position = newLocation;
	}

	public bool canPlaceBuilding()
	{
		bool canPlace = true;

		Bounds placeBounds = tempBuilding.getSelectionBounds ();
		//shorthand for the coordinates of the center of the selection bounds
		float cx = placeBounds.center.x;
		float cy = placeBounds.center.y;
		float cz = placeBounds.center.z;
		//shorthand for the coordinates of the extents of the selection box
		float ex = placeBounds.extents.x;
		float ey = placeBounds.extents.y;
		float ez = placeBounds.extents.z;

		//determine the screen coordinates for the corners of the selection bounds
		List<Vector3> corners = new List<Vector3> ();
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz + ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz - ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz + ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz + ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz - ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz + ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz - ez)));
		corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz - ez)));

		foreach (Vector3 corner in corners) {
			GameObject hitObject = workManager.findHitObject(corner);
			if (hitObject && hitObject.name != "Ground") {
				WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
				if (worldObject && placeBounds.Intersects(worldObject.getSelectionBounds())) {
					canPlace = false;
				}
			}
		}

		return canPlace;
	}

	public void startConstruction()
	{
		findingPlacement = false;
		Buildings buildings = GetComponentInChildren<Buildings> ();
		if (buildings) {
			tempBuilding.transform.parent = buildings.transform;
		}
		tempBuilding.setPlayer ();
		tempBuilding.setColliders (true);
		tempCreator.setBuilding (tempBuilding);
		tempBuilding.startConstruction ();
	}

	public void cancelBuildingPlacement()
	{
		findingPlacement = false;
		Destroy (tempBuilding.gameObject);
		tempBuilding = null;
		tempCreator = null;
	}
}

