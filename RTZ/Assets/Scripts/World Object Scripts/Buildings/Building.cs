﻿using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Building : WorldObject 
{
	//public variables
	public float maxBuildProgress;
	public Texture2D rallyPointImage, sellImage;

	//protected variables
	protected Queue< string > buildQueue;
	protected Vector3 rallyPoint;

	//private variables
	private float currentBuildProgress = 0.0f;
	private Vector3 spawnPoint;
	private bool needsBuilding = false;

	protected override void Awake()
	{
		base.Awake ();

		float spawnX;
		float spawnZ;
		buildQueue = new Queue< string > ();
		spawnX = selectionBounds.center.x + transform.forward.x * selectionBounds.extents.x + transform.forward.x * 10;
		spawnZ = selectionBounds.center.z + transform.forward.z + selectionBounds.extents.z + transform.forward.z * 10;
		spawnPoint = new Vector3 (spawnX, 0.0f, spawnZ);

		rallyPoint = spawnPoint;
	}

	protected override void Start()
	{
		base.Start ();
	}

	protected override void Update()
	{
		base.Update ();
		processBuildQueue ();
	}

	protected override void OnGUI()
	{
		base.OnGUI ();
		if (needsBuilding) {
			DrawBuildProgress ();
		}
	}

	public override void rightMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		base.rightMouseClick (hitObject, hitPoint, controller);

		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected) {
			if (hitObject.name == "Ground") {
				setRallyPoint (hitPoint);
			}
		}
	}

	public override void leftMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		base.leftMouseClick (hitObject, hitPoint, controller);
		if (player && player.human && currentlySelected) {
			if (hitObject.name == "Ground") {
				if ((player.hud.getCursorState () == cursorState.RallyPoint || player.hud.getPreviousCursorState () == cursorState.RallyPoint) && hitPoint != resourceManager.InvalidPosition) {
					if (player.selectedObject) {
						player.selectedObject.SetSelection(false, playingArea);
					}
					SetSelection(true, playingArea);
					player.selectedObject = this;
					setRallyPoint (hitPoint);
				} 
			} 
		} 
	}

	protected void createUnit(string unitName)
	{
		buildQueue.Enqueue (unitName);
	}

	protected void processBuildQueue()
	{
		if (buildQueue.Count > 0) {
			currentBuildProgress += Time.deltaTime * resourceManager.buildSpeed;
			if (currentBuildProgress > maxBuildProgress) {
				if (player) {
					player.addUnit (buildQueue.Dequeue (), spawnPoint, rallyPoint, transform.rotation, this);
				}
				currentBuildProgress = 0.0f;
			}
		}
	}

	public string[] getBuildQueueValues()
	{
		string[] values = new string[buildQueue.Count];
		int pos = 0;
		foreach (string unit in buildQueue) {
			values [pos++] = unit;
		}
		return values;
	}

	public float getBuildPercentage()
	{
		return currentBuildProgress / maxBuildProgress;
	}

	public override void SetSelection(bool selected, Rect playingArea)
	{
		base.SetSelection (selected, playingArea);
		if (player) {
			RallyPoint flag = player.GetComponentInChildren<RallyPoint> ();
			if (selected) {
				if (flag && player.human && spawnPoint != resourceManager.InvalidPosition && rallyPoint != resourceManager.InvalidPosition) {
					flag.transform.localPosition = rallyPoint;
					flag.transform.forward = transform.forward;
					flag.enable ();
				}
			} else {
				if (flag && player.human) {
					flag.disable ();
				}
			}
		}
	}

	public bool hasSpawnPoint()
	{
		return spawnPoint != resourceManager.InvalidPosition && rallyPoint != resourceManager.InvalidPosition;
	}

	public override void setHoverState(GameObject hoverObject)
	{
		base.setHoverState (hoverObject);

		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected) {
			if (hoverObject.name == "Ground") {
				if (player.hud.getPreviousCursorState () == cursorState.RallyPoint) {
					player.hud.setCursorState (cursorState.RallyPoint);
				}
			}
		}
	}

	public void setRallyPoint(Vector3 position)
	{
		rallyPoint = position;
		if (player && player.human && currentlySelected) {
			RallyPoint flag = player.GetComponentInChildren<RallyPoint>();
			if (flag) {
				flag.transform.localPosition = rallyPoint;
			}
		}
	}

	public void sell()
	{
		if (player) {
			player.addResource (resourceType.Money, sellValue);
		}
		if (currentlySelected) {
			SetSelection (false, playingArea);
		}
		Destroy (this.gameObject);
	}

	public void startConstruction()
	{
		setSpawnPoint ();
		calculateBounds ();
		needsBuilding = true;
		hitPoints = 0;
	}

	public bool underConstruction()
	{
		return needsBuilding;
	}

	public void construct(int amount)
	{
		hitPoints += amount;
		if (hitPoints >= maxHitPoints) {
			hitPoints = maxHitPoints;
			needsBuilding = false;
			restoreMaterials ();
			setTeamColor();
		}
	}

	/*** Private methods ***/

	private void DrawBuildProgress()
	{
		GUI.skin = resourceManager.SelectBoxSkin;
		Rect selectBox = workManager.calculateSelectionBox (selectionBounds, playingArea);
		//Draw the selection box around the currently selected object, within the bounds of the main draw area
		GUI.BeginGroup (playingArea);
		calculateCurrentHealth (0.5f, 0.99f);
		DrawHealthBar (selectBox, "Building...");
		GUI.EndGroup ();
	}

	private void setSpawnPoint()
	{
		float spawnX = selectionBounds.center.x + transform.forward.x * selectionBounds.extents.x + transform.forward.x * 10;
		float spawnZ = selectionBounds.center.z + transform.forward.z * selectionBounds.extents.z + transform.forward.z * 10;
		spawnPoint = new Vector3 (spawnX, 0.0f, spawnZ);
		rallyPoint = spawnPoint;
	}

}
