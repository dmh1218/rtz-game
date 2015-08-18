using UnityEngine;
using RTS;
using Newtonsoft.Json;

public class Harvester : Unit 
{
	//public variables
	public float capacity, collectionAmount, depositAmount;
	public Building resourceStore;

	//private variables
	private bool harvesting = false;
	private bool emptying = false;
	private float currentLoad = 0.0f;
	private float currentDeposit = 0.0f;
	private resourceType harvestType;
	private Resource resourceDeposit;
	private int loadedDepositId = -1;
	private int loadedStoreId = -1;

	/*** Game Engine Methods, all can be overriden by subclass ***/

	protected override void Start ()
	{
		base.Start ();

		if (loadedSavedValues) {
			if (player) {
				if (loadedStoreId >= 0) {
					WorldObject obj = player.getObjectForId(loadedStoreId);
					if (obj.GetType().IsSubclassOf(typeof(Building))) {
						resourceStore = (Building)obj;
					}
				}

				if (loadedDepositId >= 0) {
					WorldObject obj = player.getObjectForId(loadedDepositId);
					if (obj.GetType().IsSubclassOf(typeof(Resource))) {
						resourceDeposit = (Resource)obj;
					}
				}
			}
		} else {
			harvestType = resourceType.Unknown;
		}
	}

	protected override void Update ()
	{
		base.Update ();
		if (!rotating && !moving) {
			if (harvesting || emptying) {
				Arms[] arms = GetComponentsInChildren<Arms> ();
				foreach (Arms arm in arms) {
					arm.GetComponent<Renderer> ().enabled = true;
				}
				if (harvesting) {
					collect();
					if (currentLoad >= capacity || resourceDeposit.isEmpty()) {
						//make sure that we have a whole number to avoid bugs
						//caused by floating point numbers
						currentLoad = Mathf.Floor(currentLoad);
						harvesting = false;
						emptying = true;
						foreach (Arms arm in arms) {
							arm.GetComponent<Renderer>().enabled = false;
						}
						startMove (resourceStore.transform.position, resourceStore.gameObject);
					}
				} else {
					deposit();
					if (currentLoad <= 0) {
						emptying = false;
						foreach (Arms arm in arms) {
							arm.GetComponent<Renderer>().enabled = false;
						}
						if (!resourceDeposit.isEmpty()) {
							harvesting = true;
							startMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
						}
					}
				}

			}
		}
	}
	
	protected override void handleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		base.handleLoadedProperty (reader, propertyName, readValue);

		switch (propertyName) {
		case "Harvesting":
			harvesting = (bool)readValue;
			break;
		case "Emptying":
			emptying = (bool)readValue;
			break;
		case "CurrentLoad":
			currentLoad = (float)loadManager.convertToFloat(readValue);
			break;
		case "CurrentDeposit":
			currentDeposit = (float)loadManager.convertToFloat(readValue);
			break;
		case "HarvestType":
			harvestType = workManager.getResourceType ((string)readValue);
			break;
		case "ResourceDepositId":
			loadedDepositId = (int)(System.Int64)readValue;
			break;
		case "ResourceStoreId":
			loadedStoreId = (int)(System.Int64)readValue;
			break;
		default:
			break;
		}
	}

	protected override bool shouldMakeDecision()
	{
		if (harvesting || emptying) {
			return false;
		}

		return base.shouldMakeDecision ();
	}

	/*** Public Methods ***/

	public override void saveDetails(JsonWriter writer)
	{
		base.saveDetails (writer);
		
		saveManager.writeBoolean (writer, "Harvesting", harvesting);
		saveManager.writeBoolean (writer, "Emptying", emptying);
		saveManager.writeFloat (writer, "CurrentLoad", currentLoad);
		saveManager.writeFloat (writer, "CurrentDeposit", currentDeposit);
		saveManager.writeString (writer, "HarvestType", harvestType.ToString ());

		if (resourceDeposit) {
			saveManager.writeInt (writer, "ResourceDepositId", resourceDeposit.objectId);
		}

		if (resourceStore) {
			saveManager.writeInt (writer, "ResourceStoreId", resourceStore.objectId);
		}
	}

	public override void setHoverState(GameObject hoverObject)
	{
		base.setHoverState (hoverObject);

		//only handle input if owned by human player and currently selected
		if (player && player.human && currentlySelected) {
			if (!workManager.objectIsGround(hoverObject)) {
				Resource resource = hoverObject.transform.parent.GetComponent<Resource> ();
				if (resource && !resource.isEmpty ()) {
					player.hud.setCursorState (cursorState.Harvest);
				}
			}
		}
	}

	public override void rightMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		base.rightMouseClick (hitObject, hitPoint, controller);

		//only handle input if owne by a human player and currently selected
		if (player && player.human) {
			if (!workManager.objectIsGround(hitObject)) {
				Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
				if (resource && !resource.isEmpty ()) {
					Debug.Log("resource");
					startHarvest (resource);
				}
			} else {
				stopHarvest ();
			}
		}
	}

	protected override void drawSelectionBox (Rect selectBox)
	{
		base.drawSelectionBox (selectBox);
		float percentFull = currentLoad / capacity;
		float maxHeight = selectBox.height - 4;
		float height = maxHeight * percentFull;
		float leftPos = selectBox.x + selectBox.width - 7;
		float topPos = selectBox.y + 2 + (maxHeight - height);
		float width = 5;
		Texture2D resourceBar = resourceManager.getResourceHealthBar (harvestType);
		if (resourceBar) {
			GUI.DrawTexture (new Rect (leftPos, topPos, width, height), resourceBar);
		}
	}

	public override void setBuilding (Building creator)
	{
		base.setBuilding (creator);
		resourceStore = creator;
	}

	/*** Private Methods ***/

	private void startHarvest(Resource resource)
	{
		resourceDeposit = resource;
		startMove (resource.transform.position, resource.gameObject);
		//we can only collect one resource at a time, other resources are lost
		if (harvestType == resourceType.Unknown || harvestType != resource.getResourceType ()) {
			harvestType = resource.getResourceType ();
			currentLoad = 0.0f;
		}
		harvesting = true;
		emptying = false;
	}

	private void stopHarvest()
	{

	}

	private void collect()
	{
		float collect = collectionAmount * Time.deltaTime;
		//make sure that the harvester cannot collect more than it can carry
		if (currentLoad + collect > capacity) {
			collect = capacity - currentLoad;
		}
		resourceDeposit.remove (collect);
		currentLoad += collect;
	}

	private void deposit()
	{
		currentDeposit += depositAmount * Time.deltaTime;
		int deposit = Mathf.FloorToInt (currentDeposit);
		if (deposit >= 1) {
			if (deposit > currentLoad) {
				deposit = Mathf.FloorToInt (currentLoad);
			}
			currentDeposit -= deposit;
			currentLoad -= deposit;
			resourceType depositType = harvestType;
			if (harvestType == resourceType.Ore) {
				depositType = resourceType.Salvage;
			}
			player.addResource (depositType, deposit);
		}
	}
}
