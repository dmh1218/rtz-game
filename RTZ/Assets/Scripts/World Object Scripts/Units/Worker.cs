using UnityEngine;

public class Worker : Unit {
	//public variables
	public int buildSpeed;

	//private variables
	private Building currentProject;
	private bool building = false;
	private float amountBuilt = 0.0f;

	/*** Game Engine methods, all can be overriden by subclass ***/

	protected override void Start()
	{
		base.Start ();
		actions = new string[] {"Refinery", "WarFactory"};
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
		if (player && player.human && currentlySelected && hitObject && hitObject.name != "Ground") {
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
