using UnityEngine;
using System.Collections;
using RTS;
using Newtonsoft.Json;

public class Unit : WorldObject 
{
	protected bool moving;
	protected bool rotating;
	protected bool finishedMove = false;

	private Vector3 destination;
	private Quaternion targetRotation;
	private GameObject destinationTarget;
	private int loadedDestinationTargetId;

	//public variables
	public float rotateSpeed;
	public float moveSpeed;
	public int foodCost;
	public float decreaseFoodTime;
	public NavMeshAgent agent;

	/*** Game Engine methods, all can be overridden by subclass ***/

	protected override void Awake()
	{
		base.Awake ();
	}

	protected override void Start()
	{
		base.Start ();

		InvokeRepeating ("decreaseFood", decreaseFoodTime, decreaseFoodTime);
		agent = GetComponent<NavMeshAgent> ();

		if (player && loadedSavedValues && loadedDestinationTargetId >= 0) {
			destinationTarget = player.getObjectForId (loadedDestinationTargetId).gameObject;
		}
	}

	protected override void Update()
	{
		base.Update ();

		if (rotating) {
			turnToTarget ();
		} else if (moving) {
			makeMove ();
		}
	}

	protected override void OnGUI()
	{
		base.OnGUI ();
	}

	protected override void handleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		base.handleLoadedProperty (reader, propertyName, readValue);

		switch (propertyName) {
		case "Moving":
			moving = (bool)readValue;
			break;
		case "Rotating":
			rotating = (bool)readValue;
			break;
		case "Destination":
			destination = loadManager.loadVector(reader);
			break;
		case "TargetRotation":
			targetRotation = loadManager.loadQuaternion(reader);
			break;
		case "DestinationTargetId":
			loadedDestinationTargetId = (int)(System.Int64)readValue;
			break;
		default:
			break;
		}
	}

	protected override bool shouldMakeDecision()
	{
		if (moving || rotating) {
			return false;
		}
		return base.shouldMakeDecision ();
	}

	//override function to save unique aspects for units
	public override void saveDetails(JsonWriter writer)
	{
		base.saveDetails (writer);

		saveManager.writeBoolean (writer, "Moving", moving);
		saveManager.writeBoolean (writer, "Rotating", rotating);
		saveManager.writeVector (writer, "Destination", destination);
		saveManager.writeQuaternion (writer, "TargetRotation", targetRotation);

		if (destinationTarget) {
			WorldObject destinationObject = destinationTarget.GetComponent<WorldObject> ();
			if (destinationObject) {
				saveManager.writeInt (writer, "DestinationTargetId", destinationObject.objectId);
			}
		}
	}

	public override void setHoverState(GameObject hoverObject)
	{
		base.setHoverState (hoverObject);
		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected) {
			bool moveHover = false;
			if (workManager.objectIsGround(hoverObject)) {
				moveHover = true;
			} else {
				Resource resource = hoverObject.transform.parent.GetComponent<Resource>();
				if (resource && resource.isEmpty()) {
					moveHover = true;
				}
			}
			if (moveHover) {
				player.hud.setCursorState (cursorState.Move);
			}

		}
	}

	public override void rightMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		base.rightMouseClick (hitObject, hitPoint, controller);

		if (player && player.human && currentlySelected) {
			bool clickedOnEmptyResource = false;

			if (hitObject.transform.parent) {
				Resource resource = hitObject.transform.parent.GetComponent<Resource>();
				if (resource && resource.isEmpty()) {
					clickedOnEmptyResource = true;
				}
			}

			if ((workManager.objectIsGround(hitObject) || clickedOnEmptyResource) && hitPoint != resourceManager.InvalidPosition) {
				float x = hitPoint.x;
				//make sure that the unit stays on top of the surface it is on
				float y = hitPoint.y + player.selectedObject.transform.position.y;
				float z = hitPoint.z;
				Vector3 destination = new Vector3 (x, y, z);
				agent.SetDestination(destination);
				startMove (destination);
			}
		}
	}



	public virtual void startMove(Vector3 destination)
	{
		this.destination = destination;
		destinationTarget = null;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		rotating = true;
		moving = false;
	}

	public void startMove (Vector3 destination, GameObject destinationTarget)
	{
		startMove (destination);
		this.destinationTarget = destinationTarget;
	}

	private void turnToTarget()
	{
		transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotateSpeed);
		//sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
		Quaternion inverseTargetRotation = new Quaternion (-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
			rotating = false;
			moving = true;
		}
		calculateBounds ();
		if (destinationTarget) {
			calculateTargetDestination ();
		}
	}

	private void calculateTargetDestination()
	{
		//calculate number of unit vectors from unit center to unit edge of bounds
		Vector3 originalExtents = selectionBounds.extents;
		Vector3 normalExtents = originalExtents;
		normalExtents.Normalize ();
		float numberOfExtents = originalExtents.x / normalExtents.x;
		int unitShift = Mathf.FloorToInt (numberOfExtents);

		//calculate number of unit vectors from target center to target edge of bounds
		WorldObject worldObject = destinationTarget.GetComponent<WorldObject> ();
		if (worldObject) {
			originalExtents = worldObject.getSelectionBounds ().extents;
		} else {
			originalExtents = new Vector3 (0.0f, 0.0f, 0.0f);
		}
		normalExtents = originalExtents;
		normalExtents.Normalize ();
		numberOfExtents = originalExtents.x / normalExtents.x;
		int targetShift = Mathf.FloorToInt (numberOfExtents);

		//calculate number of unit vectors between unit center and destination center with bounds just touching
		int shiftAmount = targetShift + unitShift;

		//calculate direction unit needs to travel to reach destination in straight line an normalize to unit vector
		Vector3 origin = transform.position;
		Vector3 direction = new Vector3 (destination.x - origin.x, 0.0f, destination.z - origin.z);
		direction.Normalize ();

		//destination = center of destination - number of unit vectors calculated above
		//this should give us a destination where the unit will not quite collide with the target
		//giving the illusion of moving to the edge of the target and stopping
		for (int i = 0; i < shiftAmount; i++) {
			destination -= direction;
		}
		destination.y = destinationTarget.transform.position.y;

		destinationTarget = null;
	}

	private void makeMove()
	{
		agent.destination = destination;
		if (transform.position == destination) {
			Debug.Log("reached destination");
			moving = false;
			movingIntoPosition = false;
		}
		calculateBounds ();
	}

	private void moveToObject()
	{
		Debug.Log (destinationTarget.transform.position.magnitude);
		agent.destination = destinationTarget.transform.position;
		makeMove ();
	}

	public virtual void setBuilding(Building creator)
	{
		//specific initialization for a unit can be specified here
	}

	private void decreaseFood()
	{
		player.removeResource (resourceType.Food, foodCost);
	}
}
