using UnityEngine;
using System.Collections;
using RTS;

public class Unit : WorldObject 
{
	protected bool moving;
	protected bool rotating;

	private Vector3 destination;
	private Quaternion targetRotation;
	private GameObject destinationTarget;

	public float rotateSpeed;
	public float moveSpeed;

	/*** Game Engine methods, all can be overridden by subclass ***/

	protected override void Awake()
	{
		base.Awake ();
	}

	protected override void Start()
	{
		base.Start ();
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

	public override void setHoverState(GameObject hoverObject)
	{
		base.setHoverState (hoverObject);
		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected) {
			bool moveHover = false;
			if (hoverObject.name == "Ground") {
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

//	public override void mouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
//	{
//		base.mouseClick (hitObject, hitPoint, controller);
//		//only handle input if owned by a human player and currently selected
//		if (player && player.human && currentlySelected) {
//			if (hitObject.name == "Ground" && hitPoint != resourceManager.InvalidPosition) {
//				float x = hitPoint.x;
//				//make sure that the unit stays on top of the surface it is on
//				float y = hitPoint.y + player.selectedObject.transform.position.y;
//				float z = hitPoint.z;
//				Vector3 destination = new Vector3 (x, y, z);
//				startMove (destination);
//			}
//		}
//	}

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

			if ((hitObject.name == "Ground" || clickedOnEmptyResource) && hitPoint != resourceManager.InvalidPosition) {
				float x = hitPoint.x;
				//make sure that the unit stays on top of the surface it is on
				float y = hitPoint.y + player.selectedObject.transform.position.y;
				float z = hitPoint.z;
				Vector3 destination = new Vector3 (x, y, z);
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
		transform.position = Vector3.MoveTowards (transform.position, destination, Time.deltaTime * moveSpeed);
		if (transform.position == destination) {
			moving = false;
		}
		calculateBounds ();
	}

	public virtual void setBuilding(Building creator)
	{
		//specific initialization for a unit can be specified here
	}
}
