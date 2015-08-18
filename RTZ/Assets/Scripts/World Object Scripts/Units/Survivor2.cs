using UnityEngine;
using System.Collections.Generic;
using RTS;
using Newtonsoft.Json;

public class Survivor2 : Unit 
{
	
	//public variables
	public GameObject currentTool;
	public GameObject currentWeapon;
	public float capacity, collectionAmount, depositAmount;
	public Building resourceStore;
	public GameObject hand;
	public int buildSpeed;

	
	//private variables
	private Quaternion aimRotation;
	private bool harvesting = false;
	private bool emptying = false;
	private bool setResourceStore = false;
	private bool setBuildProject = false;
	private float currentLoad = 0.0f;
	private float currentDeposit = 0.0f;
	private resourceType harvestType;
	private Resource resourceDeposit;
	private int loadedDepositId = -1;
	private int loadedStoreId = -1;
	private GameObject currentGear;	
	private Building currentProject;
	private bool building = false;
	private float amountBuilt = 0.0f;
	
	protected override void Start () 
	{
		base.Start ();

		currentGear = currentWeapon;

		actions = new string[] {"Refinery", "Barricade", "Turret"};


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

		if (aiming) {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, aimRotation, weaponAimSpeed);
			calculateBounds ();
			Quaternion inverseAimRotation = new Quaternion (-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
			if (transform.rotation == aimRotation || transform.rotation == inverseAimRotation) {
				aiming = false;
			}
		}

		if (!rotating && !moving) {
			if (harvesting || emptying) {
				if (harvesting) {
					collect();
					if (currentLoad >= capacity || resourceDeposit.isEmpty()) {
						currentLoad = Mathf.Floor(currentLoad);
						harvesting = false;
						emptying = true;
						startMove (resourceStore.transform.position, resourceStore.gameObject);
					}
				} else {
					deposit();
					if (currentLoad <= 0) {
						emptying = false;
						if (!resourceDeposit.isEmpty()) {
							harvesting = true;
							startMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
						} else {
							FoodCrate foodCrate = resourceDeposit.GetComponent<FoodCrate>();
							if (foodCrate) {
								crateEmpty(foodCrate.gameObject);
							}
						}
					}
				}
			} else if (building && currentProject && currentProject.underConstruction ()) {
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

	protected override bool shouldMakeDecision()
	{
		if (harvesting || emptying || building) {
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
				setBuildProject = true;
				setBuilding (closestBuilding);
				setBuildProject = false;
			}
		}
	}
	
	protected override void aimAtTarget()
	{
		base.aimAtTarget ();

		aimRotation = Quaternion.LookRotation (target.transform.position - transform.position);
	}
	
	protected override void useWeapon()
	{
		base.useWeapon ();
		
		Vector3 spawnPoint = transform.position;
		spawnPoint.x += (1f * transform.forward.x);
		spawnPoint.y += 2f;
		spawnPoint.z += (2.1f * transform.forward.z);
		GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("Bullet"), spawnPoint, transform.rotation);
		//GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("Bullet"), currentWeapon.transform.position, transform.rotation);

		Projectile projectile = gameObject.GetComponentInChildren<Projectile> ();

		projectile.setRange (0.9f * weaponRange);
		projectile.setTarget (target);
	}


	/*** PUBLIC METHODS ***/

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
		bool doBase = true;


		if (player && player.human && currentlySelected && hitObject && !workManager.objectIsGround(hitObject)) {
			Building building = hitObject.transform.parent.GetComponent<Building> ();
			if (building) {
				if (building.underConstruction ()) {
					setBuildProject = true;
					setBuilding (building);
					setBuildProject = false;
					doBase = false;
				}
			}
		}
		if (!doBase) {
			//base.rightMouseClick (hitObject, hitPoint, controller);
			return;
		}


		base.rightMouseClick (hitObject, hitPoint, controller);


		//only handle input if owne by a human player and currently selected
		if (player && player.human) {
			if (!workManager.objectIsGround(hitObject)) {
				//if right clicked on a food crate
				FoodCrate foodCrate = gameObject.GetComponentInChildren<FoodCrate>();
				if (foodCrate) {
					dropCrate(foodCrate.gameObject);
				}

				//if right clicked on a resource deposit
				Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
				if (resource && !resource.isEmpty ()) {
					startHarvest (resource);
				} else {
					currentWeapon.SetActive(true);
					currentTool.SetActive(false);
					currentGear = currentWeapon;
				}

			} else {
				stopHarvest ();
			}
		}

	}

	public override void setBuilding (Building bldg)
	{
		base.setBuilding (bldg);
		if (setResourceStore) {
			resourceStore = bldg;
		} else if (setBuildProject) {
			currentProject = bldg;
			startMove (currentProject.transform.position, currentProject.gameObject);
			building = true;
		}
	}
	
	public override bool canAttack()
	{
		return true;
	}

	public override void PerformAction(string actionToPerform)
	{
		base.PerformAction (actionToPerform);
		createBuilding (actionToPerform);
	}

	public override void startMove(Vector3 destination)
	{
		base.startMove (destination);

		amountBuilt = 0.0f;
		building = false;
	}

	/*** PRIVATE METHODS ***/

	private void startHarvest(Resource resource)
	{
		currentWeapon.SetActive (false);
		currentTool.SetActive (true);
		currentGear = currentTool;

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
		harvesting = false;
		emptying = false;
	}
	
	private void collect()
	{
		float collect = 0;

		FoodCrate foodCrate = resourceDeposit.GetComponentInChildren<FoodCrate> ();
		if (foodCrate) {
			pickUpCrate (foodCrate.gameObject);
			collect = collectionAmount;
			//collect = foodCrate.capacity;
		} else {
			collect = collectionAmount * Time.deltaTime;
		}

		if (foodCrate) {
			collect = foodCrate.capacity;
		} else if (currentLoad + collect > capacity) {
			//make sure that the harvester cannot collect more than it can carry
			collect = capacity - currentLoad;
		}
		resourceDeposit.remove (collect);
		currentLoad += collect;
	}

	private void deposit()
	{
		FoodCrate foodCrate = GetComponentInChildren<FoodCrate> ();

		if (foodCrate) {
			currentDeposit = depositAmount;
			//currentDeposit = foodCrate.capacity;
		} else {
			currentDeposit += depositAmount * Time.deltaTime;
		}

		int deposit = Mathf.FloorToInt (currentDeposit);
		if (deposit >= 1) {
			if (foodCrate) {
				currentDeposit = foodCrate.capacity;
			} else if (deposit > currentLoad) {
				deposit = Mathf.FloorToInt (currentLoad);
			}
			currentDeposit -= deposit;
			currentLoad -= deposit;
			resourceType depositType = harvestType;
			if (harvestType == resourceType.Food) {
				depositType = resourceType.Food;
			}

			player.addResource (depositType, deposit);
		}
	}	

	private void pickUpCrate(GameObject crate)
	{
		//float handHeight = 1.7f;
		//float handDistance = 1.3f;

		//crate.transform.position = new Vector3 (gameObject.transform.position.x - handDistance, handHeight, gameObject.transform.position.z);
		crate.transform.position = hand.transform.position;

		crate.transform.parent = gameObject.transform;

		currentGear.SetActive (false);
	}

	private void crateEmpty(GameObject crate)
	{
		Destroy (crate);
		currentGear.SetActive (true);
	}

	private void dropCrate(GameObject crate)
	{
		crate.transform.parent = null;
		crate.transform.position = new Vector3 (crate.transform.position.x, 0.0f, crate.transform.position.z);
		currentGear.SetActive (true);
	}

	private bool carryingCrate()
	{
		FoodCrate foodCrate = GetComponentInChildren<FoodCrate> ();
		if (foodCrate) {
			return true;
		} else {
			return false;
		}
	}

	private void createBuilding(string buildingName)
	{
		Vector3 buildPoint = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 10);
		if (player) {
			player.createBuilding (buildingName, buildPoint, this, playingArea);
		}
	}
}