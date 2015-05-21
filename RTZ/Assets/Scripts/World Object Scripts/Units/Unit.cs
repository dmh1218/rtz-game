using UnityEngine;
using System.Collections;
using RTS;

public class Unit : WorldObject 
{
	protected bool moving;
	protected bool rotating;

	private Vector3 destination;
	private Quaternion targetRotation;

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
			if (hoverObject.name == "Ground") {
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
			if (hitObject.name == "Ground" && hitPoint != resourceManager.InvalidPosition) {
				float x = hitPoint.x;
				//make sure that the unit stays on top of the surface it is on
				float y = hitPoint.y + player.selectedObject.transform.position.y;
				float z = hitPoint.z;
				Vector3 destination = new Vector3 (x, y, z);
				startMove (destination);
			}
		}
	}

	public void startMove(Vector3 destination)
	{
		this.destination = destination;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		rotating = true;
		moving = false;
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
	}

	private void makeMove()
	{
		transform.position = Vector3.MoveTowards (transform.position, destination, Time.deltaTime * moveSpeed);
		if (transform.position == destination) {
			moving = false;
		}
		calculateBounds ();
	}
}
