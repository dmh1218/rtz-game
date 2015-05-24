using UnityEngine;
using RTS;

public class Harvester : Unit 
{
	//public variables
	public float capacity, collectionAmount, depositAmount;
	public Building resourceStore;

	//private variables
	private bool harvesting = false, emptying = false;
	private float currentLoad = 0.0f;
	private float currentDeposit = 0.0f;
	private resourceType harvestType;
	private Resource resourceDeposit;

	/*** Game Engine Methods, all can be overriden by subclass ***/

	protected override void Start ()
	{
		base.Start ();
		harvestType = resourceType.Unknown;
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

	/*** Public Methods ***/

	public override void setHoverState(GameObject hoverObject)
	{
		base.setHoverState (hoverObject);

		//only handle input if owned by human player and currently selected
		if (player && player.human && currentlySelected) {
			if (hoverObject.name != "Ground") {
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
			if (hitObject.name != "Ground") {
				Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
				if (resource && !resource.isEmpty ()) {
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
				depositType = resourceType.Money;
			}
			player.addResource (depositType, deposit);
		}
	}
}
