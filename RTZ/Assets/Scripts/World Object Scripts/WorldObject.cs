using UnityEngine;
using System.Collections;
using RTS;

public class WorldObject : MonoBehaviour 
{
	public string objectName;
	public Texture2D buildImage;
	public int cost;
	public int sellValue;
	public int hitPoints;
	public int maxHitPoints;
	
	Rect selectBox;
	
	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;
	protected Bounds selectionBounds;
	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	
	protected virtual void Awake()
	{
		selectionBounds = resourceManager.InvalidBounds;
		calculateBounds ();
	}
	
	protected virtual void Start()
	{
		player = transform.root.GetComponentInChildren<Player> ();
	}
	
	protected virtual void Update()
	{
		
	}
	
	protected virtual void OnGUI()
	{
		if (currentlySelected) {
			drawSelection ();
		}
	}
	
	public virtual void SetSelection(bool selected, Rect playingArea)
	{
		currentlySelected = selected;
		if (selected) {
			this.playingArea = playingArea;
		}
	}
	
	public string[] GetActions()
	{
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform)
	{
		//it is up to children with specific actions to determine what to do with each of those actions
	}
	
	public virtual void mouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) 
	{
		//only handle input if currently selected
		if (currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject> ();
			//clicked on another selectable object
			if (worldObject) {
				changeSelection (worldObject, controller);
			}
		}
	}

	public virtual void rightMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{

	}

	public virtual void leftMouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
	{
		//only handle input if currently selected
		if (currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject> ();
			//clicked on another selectable object
			if (worldObject) {
				changeSelection (worldObject, controller);
			}
		}
	}
	
	private void changeSelection(WorldObject worldObject, Player controller)
	{
		//this should be called by the following line, but there is an outside chance it will not
		SetSelection (false, playingArea);
		if (controller.selectedObject) {
			controller.selectedObject.SetSelection (false, playingArea);
		}
		controller.selectedObject = worldObject;
		worldObject.SetSelection (true, controller.hud.getPlayingArea());
	}
	
	private void drawSelection()
	{
		GUI.skin = resourceManager.SelectBoxSkin;
		selectBox = workManager.calculateSelectionBox (selectionBounds, playingArea);
		//Draw the selection box around the currently selected object, within the bounds of the playing area
		GUI.BeginGroup (playingArea);
		drawSelectionBox (selectBox);
		GUI.EndGroup ();
	}
	
	public void calculateBounds()
	{
		selectionBounds = new Bounds (transform.position, Vector3.zero);
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			selectionBounds.Encapsulate (r.bounds);
		}
	}
	
	protected virtual void drawSelectionBox(Rect selectBox)
	{
		GUI.Box (selectBox, "");
	}
	
	public virtual void setHoverState(GameObject hoverObject) 
	{
		//only handle input if owned by a human player and currently selected
		if (player && player.human && currentlySelected) {
			if (hoverObject.name != "Ground") {
				player.hud.setCursorState (cursorState.Select);
			}
		}
	}

	public bool isOwnedBy (Player owner)
	{
		if (player && player.Equals (owner)) {
			return true;
		} else {
			return false;
		}
	}
	
}
