using UnityEngine;
using System.Collections.Generic;
using RTS;
using Newtonsoft.Json;

public class Worker : Unit {
	//public variables
	public int buildSpeed;

	//private variables
	private Building currentProject;
	private bool building = false;
	private float amountBuilt = 0.0f;
	private int loadedProjectId = -1;

	/*** Game Engine methods, all can be overriden by subclass ***/

	protected override void Start()
	{
		base.Start ();

		actions = new string[] {"Refinery", "WarFactory", "Turret"};
		if (player && loadedSavedValues && loadedProjectId >= 0) {
			WorldObject obj = player.getObjectForId (loadedProjectId);
			if (obj.GetType ().IsSubclassOf (typeof(Building))) {
				currentProject = (Building)obj;

			}
		}
	}

	protected override void Update()
	{
		base.Update ();

		if (!moving && !rotating) {
			if (building && currentProject && currentProject.underConstruction ()) {
				amountBuilt += buildSpeed * Time.deltaTime;
				int amount = Mathf.FloorToInt (amountBuilt);
				if (amount > 0) {
					amountBuilt -= amount;
					currentProject.construct(amount);
					if (!currentProject.underConstruction ()) {
						building = false;
					}
				}
			}
		}
	}

	protected override void handleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		base.handleLoadedProperty (reader, propertyName, readValue);

		switch (propertyName) {
		case "Building":
			building = (bool)readValue;
			break;
		case "AmountBuilt":
			amountBuilt = (float)loadManager.convertToFloat(readValue);
			break;
		case "CurrentProjectId":
			loadedProjectId = (int)(System.Int64)readValue;
			break;
		default:
			break;
		}
	}

	protected override bool shouldMakeDecision()
	{
		if (building) {
			return false;
		}
		return base.shouldMakeDecision ();
	}

	protected override void decideWhatToDo()
	{
		base.decideWhatToDo ();

		List<WorldObject> buildings = new List<WorldObject> ();

		foreach (WorldObject nearbyObject in nearbyObjects) {
			if (nearbyObject.getPlayer () != player) {
				continue;
			}
			Building nearbyBuilding = nearbyObject.GetComponent<Building> ();
			if (nearbyBuilding && nearbyBuilding.underConstruction ()) {
				buildings.Add (nearbyObject);
			}
		}

		WorldObject nearestObject = workManager.findNearestWorldObject (buildings, transform.position);

		if (nearestObject) {
			Building closestBuilding = nearestObject.GetComponent<Building> ();
			if (closestBuilding) {
				setBuilding (closestBuilding);
			}
		}
	}

	/*** Public methods ***/


	public override void setBuilding(Building project)
	{
		base.setBuilding (project);
		currentProject = project;
		startMove (currentProject.transform.position, currentProject.gameObject);
		building = true;
	}

	public override void PerformAction(string actionToPerform)
	{
		base.PerformAction (actionToPerform);
		createBuilding (actionToPerform);
	}

	public override void rightMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		bool doBase = true;

		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected && hitObject && !workManager.objectIsGround(hitObject)) {
			Building building = hitObject.transform.parent.GetComponent<Building> ();
			if (building) {
				if (building.underConstruction ()) {
					setBuilding (building);
					doBase = false;
				}
			}
		}
		if (doBase) {
			base.rightMouseClick (hitObject, hitPoint, controller);
		}
	}


	public override void startMove(Vector3 destination)
	{
		base.startMove (destination);
		amountBuilt = 0.0f;
		building = false;
	}

	/*** Private methods ***/

	private void createBuilding(string buildingName)
	{
		Vector3 buildPoint = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 10);
		if (player) {
			player.createBuilding (buildingName, buildPoint, this, playingArea);
		}
	}

}
