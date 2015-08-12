using UnityEngine;
using System.Collections.Generic;
using RTS;
using Newtonsoft.Json;

public class Player : MonoBehaviour 
{
	//public variables
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject selectedObject { get; set; }
	public int startMoney, startMoneyLimit, startFood, startFoodLimit;
	public Material notAllowedMaterial, allowedMaterial;
	public Color teamColor;

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

	/***PRIVATE METHODS***/

	//create new resources at start of game
	private Dictionary< resourceType, int > initResourceList()
	{
		Dictionary< resourceType, int > list = new Dictionary< resourceType, int > ();
		list.Add (resourceType.Money, 0);
		list.Add (resourceType.Food, 0);
		return list;
	}

	//add the starting resource limit for each resource at start of game
	private void addStartResourceLimits()
	{
		incrementResourceLimit (resourceType.Money, startMoneyLimit);
		incrementResourceLimit (resourceType.Food, startFoodLimit);
	}

	//add the starting amount of resources for each resource at start of game
	private void addStartResources()
	{
		addResource (resourceType.Money, startMoney);
		addResource (resourceType.Food, startFood);
	}

	//load resources details for player
	private void loadResources(JsonTextReader reader)
	{
		if (reader == null) {
			return;
		}

		string currValue = "";
		while (reader.Read()) {
			if (reader.Value != null) {
				if (reader.TokenType == JsonToken.PropertyName) {
					currValue = (string)reader.Value;
				} else {
					switch(currValue) {
					case "Money":
						startMoney = (int)(System.Int64)reader.Value;
						break;
					case "Money_Limit":
						startMoneyLimit = (int)(System.Int64)reader.Value;
						break;
					case "Power":
						//startPower = (int)(System.Int64)reader.Value;
						break;
					case "Power_Limit":
						//startPowerLimit = (int)(System.Int64)reader.Value;
						break;
					default:
						break;
					}
				}
			} else if (reader.TokenType == JsonToken.EndArray) {
				return;
			}
		}
	}

	//load building details for player
	private void loadBuildings(JsonTextReader reader)
	{
		if (reader == null) {
			return;
		}

		Buildings buildings = GetComponentInChildren<Buildings> ();
		string currValue = "";
		string type = "";

		while (reader.Read()) {
			if (reader.Value != null) {
				if (reader.TokenType == JsonToken.PropertyName) {
					currValue = (string)reader.Value;
				} else if (currValue == "Type") {
					type = (string)reader.Value;
					GameObject newObject = (GameObject)GameObject.Instantiate(resourceManager.getBuilding(type));
					Building building = newObject.GetComponent<Building>();
					building.loadDetails(reader);
					building.transform.parent = buildings.transform;
					building.setPlayer();
					building.setTeamColor();
					if (building.underConstruction()) {
						building.setTransparentMaterial(allowedMaterial, true);
					}
				}
			} else if (reader.TokenType == JsonToken.EndArray) {
				return;
			}
		}
	}

	//load unit details for player
	private void loadUnits(JsonTextReader reader)
	{
		if (reader == null) {
			return;
		}

		Units units = GetComponentInChildren<Units> ();
		string currValue = "";
		string type = "";

		while (reader.Read()) {
			if (reader.Value != null) {
				if (reader.TokenType == JsonToken.PropertyName) {
					currValue = (string)reader.Value;
				} else if (currValue == "Type") {
					type = (string)reader.Value;
					GameObject newObject = (GameObject)GameObject.Instantiate(resourceManager.getUnit(type));
					Unit unit = newObject.GetComponent<Unit>();
					unit.loadDetails(reader);
					unit.transform.parent = units.transform;
					unit.setPlayer();
					unit.setTeamColor();
				}
			} else if (reader.TokenType == JsonToken.EndArray) {
				return;
			}
		}
	}

	/***PUBLIC METHODS***/

	//add given amount of resources to given resource type
	public void addResource (resourceType type, int amount)
	{
		resources [type] += amount;

//		int currAmount = resources [type];
//		int currLimit = resourceLimits [type];
//
//		if (currAmount < currLimit) {
//			//resource limit is greater than the current resource amount
//			resources [type] += amount;
//			currAmount = resources [type];
//			if (currAmount > currLimit) {
//				resources [type] = currLimit;
//			}
//			//return true;
//		} else {
//			//stop harvesting
//			//return false;
//		}
	}

	//increase the limit for the given resource type by the given amount
	public void incrementResourceLimit (resourceType type, int amount)
	{
		resourceLimits [type] += amount;
	}

	//create a new instance of the given unit, controlled by the player, at the spawn point and send it to the rally point
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
			unitObject.objectId = resourceManager.getNewObjectId();
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
			tempBuilding.objectId = resourceManager.getNewObjectId();
			tempCreator = creator;
			findingPlacement = true;
			tempBuilding.setTransparentMaterial (notAllowedMaterial, true);
			tempBuilding.setColliders (false);
			tempBuilding.setPlayingArea (playingArea);
			tempBuilding.hitPoints = 0;
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
			if (hitObject && !workManager.objectIsGround(hitObject)) {
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
		removeResource (resourceType.Money, tempBuilding.cost);
	}

	public void cancelBuildingPlacement()
	{
		findingPlacement = false;
		Destroy (tempBuilding.gameObject);
		tempBuilding = null;
		tempCreator = null;
	}

	public WorldObject getObjectForId(int id)
	{
		WorldObject[] objects = GameObject.FindObjectsOfType (typeof(WorldObject)) as WorldObject[];
		foreach (WorldObject obj in objects) {
			if (obj.objectId == id) {
				return obj;
			}
		}
		return null;
	}

	public void loadDetails(JsonTextReader reader)
	{
		if (reader == null) {
			return;
		}

		string currValue = "";
		while (reader.Read()) {
			if (reader.Value != null) {
				if (reader.TokenType == JsonToken.PropertyName) {
					currValue = (string)reader.Value;
				} else {
					switch (currValue) {
					case "Username":
						username = (string)reader.Value;
						Debug.Log("username: " + username);
						break;
					case "Human":
						human = (bool)reader.Value;
						break;
					default:
						break;
					}
				}
			} else if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray) {
				switch (currValue) {
				case "TeamColor":
					teamColor = loadManager.loadColor (reader);
					break;
				case "Resources":
					loadResources (reader);
					break;
				case "Buildings":
					loadBuildings (reader);
					break;
				case "Units":
					//loadUnits (reader);
					break;
				default:
					break;
				}
			} else if (reader.TokenType == JsonToken.EndObject) {
				return;
			}
		}
	}

	//check to see if all of the player's units and buildings have been destroyed
	public bool isDead()
	{
		Building[] buildings = GetComponentsInChildren<Building> ();
		Unit[] units = GetComponentsInChildren<Unit> ();

		if (buildings != null && buildings.Length > 0) {
			return false;
		}

		if (units != null && units.Length > 0) {
			return false;
		}

		return true;
	}

	//decrement given number of given type of resource
	public void removeResource(resourceType type, int amount)
	{
		resources [type] -= amount;
	}

	public virtual void saveDetails(JsonWriter writer)
	{
		saveManager.writeString (writer, "Username", username);
		saveManager.writeBoolean (writer, "Human", human);
		saveManager.writeColor (writer, "TeamColor", teamColor);
		saveManager.savePlayerResources (writer, resources, resourceLimits);
		saveManager.savePlayerBuildings (writer, GetComponentsInChildren<Building> ());
		saveManager.savePlayerUnits (writer, GetComponentsInChildren<Unit> ());
	}
}

