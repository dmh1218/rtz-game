using UnityEngine;
using System.Collections.Generic;
using RTS;

public class WorldObject : MonoBehaviour 
{
	//public variables
	public string objectName;
	public Texture2D buildImage;
	public int cost;
	public int sellValue;
	public int hitPoints;
	public int maxHitPoints;

	//private variables
	private Rect selectBox;
	private List <Material> oldMaterials = new List<Material> ();

	//protected variables
	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;
	protected Bounds selectionBounds;
	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	protected GUIStyle healthStyle = new GUIStyle();
	protected float healthPercentage = 1.0f;
	
	protected virtual void Awake()
	{
		selectionBounds = resourceManager.InvalidBounds;
		calculateBounds ();
	}
	
	protected virtual void Start()
	{
		setPlayer();
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
				Resource resource = hitObject.transform.parent.GetComponent<Resource>();
				if (resource && resource.isEmpty()) {
					return;
				}
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

	protected void DrawHealthBar(Rect selectBox, string label)
	{
		healthStyle.padding.top = -20;
		healthStyle.fontStyle = FontStyle.Bold;
		GUI.Label (new Rect (selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), label, healthStyle);
	}

	protected virtual void drawSelectionBox(Rect selectBox)
	{
		GUI.Box (selectBox, "");
		calculateCurrentHealth (0.35f, 0.65f);
		DrawHealthBar (selectBox, "");
	}
	
	public void calculateBounds()
	{
		selectionBounds = new Bounds (transform.position, Vector3.zero);
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			selectionBounds.Encapsulate (r.bounds);
		}
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

	public Bounds getSelectionBounds()
	{
		return selectionBounds;
	}

	protected virtual void calculateCurrentHealth(float lowSplit, float highSplit)
	{
		healthPercentage = (float)hitPoints / (float)maxHitPoints;
		if (healthPercentage > highSplit) {
			healthStyle.normal.background = resourceManager.HealthyTexture;
		} else if (healthPercentage > lowSplit) {
			healthStyle.normal.background = resourceManager.DamagedTexture;
		} else {
			healthStyle.normal.background = resourceManager.CriticalTexture;
		}
	}

	public void setColliders(bool enabled)
	{
		Collider[] colliders = GetComponentsInChildren<Collider> ();
		foreach (Collider collider in colliders) {
			collider.enabled = enabled;
		}
	}

	public void setTransparentMaterial(Material material, bool storeExistingMaterial)
	{
		if (storeExistingMaterial) {
			oldMaterials.Clear ();
		}
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			if (storeExistingMaterial) {
				oldMaterials.Add (renderer.material);
			}
			renderer.material = material;
		}
			
	}

	public void setPlayer()
	{
		player = transform.root.GetComponentInChildren<Player> ();
	}

	public void restoreMaterials()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		if (oldMaterials.Count == renderers.Length) {
			Debug.Log("put back old material");
			for (int i = 0; i < renderers.Length; i++) {
				renderers [i].material = oldMaterials [i];
				Debug.Log(oldMaterials[i].name);
			}
		}
	}

	public void setPlayingArea(Rect playingArea)
	{
		this.playingArea = playingArea;
	}
	
}
