using UnityEngine;
using System.Collections.Generic;
using RTS;

public class HUD : MonoBehaviour 
{
	public GUISkin playerDetailsSkin, ordersSkin, resourceSkin, selectBoxSkin, mouseCursorSkin;
	public Texture2D healthy, damaged, critical;
	public Texture2D buttonHover, buttonClick, smallButtonHover, smallButtonClick, buildFrame, buildMask;
	public Texture2D activeCursor, selectCursor, leftCursor, rightCursor, upCursor, downCursor, rallyPointCursor;
	public Texture2D[] moveCursors;
	public Texture2D[] attackCursors;
	public Texture2D[] harvestCursors;
	public Texture2D[] resources;
	public Texture2D[] resourceHealthBars;
	
	private static int ordersBarWidth = 150;
	private static int resourceBarHeight = 40;
	private static int selectionNameHeight = 15;

	private const int buildImageWidth = 64, buildImageHeight = 64, buildImagePadding = 8, buttonSpacing = 7, scrollBarWidth = 22;
	private const int iconWidth = 32, iconHeight = 32, textWidth = 128, textHeight = 32;

	private Player player;
	private cursorState activeCursorState;
	private int currentFrame = 0;
	private Dictionary< resourceType, int > resourceValues, resourceLimits;
	private Dictionary< resourceType, Texture2D > resourceImages;
	private WorldObject lastSelection;
	private float sliderValue;
	private int buildAreaHeight = 0;
	private cursorState previousCursorState;

	Vector3 mousePos;
	bool insideWidth;
	bool insideHeight;

	string selectionName;

	void Start()
	{
		resourceValues = new Dictionary< resourceType, int > ();
		resourceLimits = new Dictionary< resourceType, int > ();
		player = transform.root.GetComponent<Player> ();
		resourceManager.storeSelectBoxItems (selectBoxSkin, healthy, damaged, critical);
		setCursorState (cursorState.Select);

		resourceImages = new Dictionary<resourceType, Texture2D> ();
		for (int i = 0; i < resources.Length; i++) {
			switch (resources [i].name) {
			case "Money":
				resourceImages.Add (resourceType.Money, resources [i]);
				resourceValues.Add (resourceType.Money, 0);
				resourceLimits.Add (resourceType.Money, 0);
				break;
			case "Power":
				resourceImages.Add (resourceType.Power, resources [i]);
				resourceValues.Add (resourceType.Power, 0);
				resourceLimits.Add (resourceType.Power, 0);
				break;
			default:
				break;
			}
		}

		buildAreaHeight = Screen.height - resourceBarHeight - selectionNameHeight - 2 * buttonSpacing;

		Dictionary<resourceType, Texture2D> resourceHealthBarTextures = new Dictionary<resourceType, Texture2D> ();
		for (int i = 0; i < resourceHealthBars.Length; i++) {
			switch (resourceHealthBars [i].name) {
			case "ore":
				resourceHealthBarTextures.Add (resourceType.Ore, resourceHealthBars [i]);
				break;
			default:
				break;
			}
		}
		resourceManager.setResourceHealthBarTextures (resourceHealthBarTextures);
	}

	void OnGUI()
	{
		if (player && player.human) {
			DrawPlayerDetails();
			DrawOrdersBar();
			DrawResourceBar();
			DrawMouseCursor();
		}
	}

	private void DrawPlayerDetails()
	{
		GUI.skin = playerDetailsSkin;
		GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
		float height = resourceManager.textHeight;
		float leftPos = resourceManager.padding;
		float topPos = Screen.height - height - resourceManager.padding;
		float minWidth = 0, maxWidth = 0;
		string playerName = playerManager.getPlayerName ();
		playerDetailsSkin.GetStyle ("label").CalcMinMaxWidth (new GUIContent (playerName), out minWidth, out maxWidth);
		GUI.Label (new Rect (leftPos, topPos, maxWidth, height), playerName);
		GUI.EndGroup ();
	}

	private void DrawOrdersBar()
	{
		GUI.skin = ordersSkin;
		GUI.BeginGroup (new Rect (Screen.width - ordersBarWidth - buildImageWidth, resourceBarHeight, ordersBarWidth + buildImageWidth, Screen.height - resourceBarHeight));
		GUI.Box (new Rect (buildImageWidth + scrollBarWidth, 0, ordersBarWidth, Screen.height - resourceBarHeight), "");

		selectionName = "";
		if (player.selectedObject) {
			selectionName = player.selectedObject.objectName;

			if (player.selectedObject.isOwnedBy (player)) {
				//reset slider value if the selected object has changed
				if (lastSelection && lastSelection != player.selectedObject) {
					sliderValue = 0.0f;
				}
				DrawActions (player.selectedObject.GetActions ());
				//store the current selection
				lastSelection = player.selectedObject;

				Building selectedBuilding = lastSelection.GetComponent<Building>();
				if (selectedBuilding) {
					DrawBuildQueue(selectedBuilding.getBuildQueueValues(), selectedBuilding.getBuildPercentage());
					DrawStandardBuildingOptions(selectedBuilding);
				}
			}
		}

		if (!selectionName.Equals ("")) {
			int leftPos = buildImageWidth + scrollBarWidth / 2;
			int topPos = buildAreaHeight + buttonSpacing;
			GUI.Label (new Rect (leftPos, topPos, ordersBarWidth, selectionNameHeight), selectionName);
		}
		GUI.EndGroup ();
	}

	private void DrawResourceBar()
	{
		GUI.skin = resourceSkin;
		GUI.BeginGroup (new Rect (0, 0, Screen.width, resourceBarHeight));
		GUI.Box (new Rect (0, 0, Screen.width, resourceBarHeight), "");

		int topPos = 4, iconLeft = 4, textLeft = 20;
		DrawResourceIcon (resourceType.Money, iconLeft, textLeft, topPos);
		iconLeft += textWidth;
		textLeft += textWidth;
		DrawResourceIcon (resourceType.Power, iconLeft, textLeft, topPos);

		int padding = 7;
		int buttonWidth = ordersBarWidth - 2 * padding - scrollBarWidth;
		int buttonHeight = resourceBarHeight - 2 * padding;
		int leftPos = Screen.width - ordersBarWidth / 2 - buttonWidth / 2 + scrollBarWidth / 2;
		Rect menuButtonPosition = new Rect (leftPos, padding, buttonWidth, buttonHeight);

		if (GUI.Button (menuButtonPosition, "Menu")) {
			Time.timeScale = 0.0f;
			PauseMenu pauseMenu = GetComponent<PauseMenu> ();
			if (pauseMenu) {
				pauseMenu.enabled = true;
			}
			userInput UserInput = player.GetComponent<userInput> ();
			if (UserInput) {
				UserInput.enabled = false;
			}
		}

		GUI.EndGroup ();
	}

	private void DrawResourceIcon(resourceType type, int iconLeft, int textLeft, int topPos)
	{
		Texture2D icon = resourceImages [type];
		string text = resourceValues [type].ToString () + "/" + resourceLimits [type].ToString ();
		GUI.DrawTexture (new Rect (iconLeft, topPos, iconWidth, iconHeight), icon);
		GUI.Label (new Rect (textLeft, topPos, textWidth, textHeight), text);
	}

	private void DrawMouseCursor()
	{
		bool mouseOverHUD = !mouseInBounds() && activeCursorState != cursorState.PanRight && activeCursorState != cursorState.PanUp;
		
		if (mouseOverHUD || resourceManager.menuOpen) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;
			if (!player.isFindingBuildingLocation()) {
				GUI.skin = mouseCursorSkin;
				GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
				updateCursorAnimation();
				Rect cursorPosition = getCursorDrawPosition();
				GUI.Label(cursorPosition, activeCursor);
				GUI.EndGroup();
			}
		}
	}

	private void DrawActions(string[] actions)
	{
		GUIStyle buttons = new GUIStyle ();
		buttons.hover.background = buttonHover;
		buttons.active.background = buttonClick;
		GUI.skin.button = buttons;
		int numActions = actions.Length;
		//define the area to draw the actions inside
		GUI.BeginGroup (new Rect (buildImageWidth, 0, ordersBarWidth, buildAreaHeight));
		//draw scroll bar for the list of actions if need be
		if (numActions >= maxNumRows(buildAreaHeight)) {
			DrawSlider(buildAreaHeight, numActions / 2.0f);
		}
		//display possible actions as buttons and handle the button click for each
		for (int i = 0; i < numActions; i++) {
			int col = i % 2;
			int row = i / 2;
			Rect pos = getButtonPos(row, col);
			Texture2D action = resourceManager.getBuildImage(actions[i]);
			if (action) {
				//create button and handle the click of that button
				if (GUI.Button(pos, action)) {
					if (player.selectedObject) {
						player.selectedObject.PerformAction(actions[i]);
					}
				}
			}
		}
		GUI.EndGroup();
	}

	private void DrawSlider(int groupHeight, float numRows)
	{
		//slider goes from 0 to the number of rows that do not fit on the screen
		sliderValue = GUI.VerticalSlider (getScrollPos (groupHeight), sliderValue, 0.0f, numRows - maxNumRows (groupHeight));
	}

	private void DrawBuildQueue(string[] buildQueue, float buildPercentage)
	{
		for (int i = 0; i <buildQueue.Length; i++) {
			float topPos = i * buildImageHeight - (i + 1) * buildImagePadding;
			Rect buildPos = new Rect (buildImagePadding, topPos, buildImageWidth, buildImageHeight);
			GUI.DrawTexture (buildPos, resourceManager.getBuildImage (buildQueue [i]));
			GUI.DrawTexture (buildPos, buildFrame);
			topPos += buildImagePadding;
			float width = buildImageWidth - 2 * buildImagePadding;
			float height = buildImageHeight - 2 * buildImagePadding;
			if (i == 0) {
				//shrink the build mask on the item currently being built to give an idea of progress
				topPos += height * buildPercentage;
				height *= (1 - buildPercentage);
			}
			GUI.DrawTexture (new Rect (2 * buildImagePadding, topPos, width, height), buildMask);
		}
	}

	private void DrawStandardBuildingOptions(Building building)
	{
		GUIStyle buttons = new GUIStyle ();
		buttons.hover.background = smallButtonHover;
		buttons.active.background = smallButtonClick;
		GUI.skin.button = buttons;
		int leftPos = buildImageWidth + scrollBarWidth + buttonSpacing;
		int topPos = buildAreaHeight - buildImageHeight / 2;
		int width = buildImageWidth / 2;
		int height = buildImageHeight / 2;
		if (GUI.Button(new Rect(leftPos, topPos, width, height), building.sellImage)) {
			building.sell();
		}
		if (building.hasSpawnPoint ()) {
			leftPos += width + buttonSpacing;
			if (GUI.Button (new Rect (leftPos, topPos, width, height), building.rallyPointImage)) {
				if (activeCursorState != cursorState.RallyPoint && previousCursorState != cursorState.RallyPoint) {
					setCursorState (cursorState.RallyPoint);
				} else {
					setCursorState (cursorState.PanRight);
					setCursorState (cursorState.Select);
				}
			}
		}
	}

	public bool mouseInBounds()
	{
		//Screen coordinates start in lower-left corner of the screen
		//not the top-left of the screen like the drawing coordinates do
		mousePos = Input.mousePosition;
		insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ordersBarWidth;
		insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - resourceBarHeight;
		return insideWidth && insideHeight;
	}

	public Rect getPlayingArea()
	{
		return new Rect (0, resourceBarHeight, Screen.width - ordersBarWidth, Screen.height - resourceBarHeight);
	}

	private void updateCursorAnimation()
	{
		//sequence animation for cursor (based on more than one image for the cursor)
		//change once per second, loops through array of images
		if (activeCursorState == cursorState.Move) {
			currentFrame = (int)Time.time % moveCursors.Length;
			activeCursor = moveCursors [currentFrame];
		} else if (activeCursorState == cursorState.Attack) {
			currentFrame = (int)Time.time % attackCursors.Length;
			activeCursor = attackCursors [currentFrame];
		} else if (activeCursorState == cursorState.Harvest) {
			currentFrame = (int)Time.time % harvestCursors.Length;
			activeCursor = harvestCursors [currentFrame];
		}
	}

	private Rect getCursorDrawPosition()
	{
		//set base position for custom cursor image
		float leftPos = Input.mousePosition.x;
		//screen draw coordinates are inverted
		float topPos = Screen.height - Input.mousePosition.y;
		//adjust position based on the type of cursor being shown
		if (activeCursorState == cursorState.PanRight) {
			leftPos = Screen.width - activeCursor.width;
		} else if (activeCursorState == cursorState.PanDown) {
			topPos = Screen.height - activeCursor.height;
		} else if (activeCursorState == cursorState.Move || activeCursorState == cursorState.Select || activeCursorState == cursorState.Harvest) {
			topPos -= activeCursor.height / 2;
			leftPos -= activeCursor.width / 2;
		} else if (activeCursorState == cursorState.RallyPoint) {
			topPos -= activeCursor.height;
		}
		return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
	}

	public void setCursorState(cursorState newState)
	{
		if (activeCursorState != newState) {
			previousCursorState = activeCursorState;
		}
		activeCursorState = newState;

		switch(newState) {
		case cursorState.Select:
			activeCursor = selectCursor;
			break;
		case cursorState.Attack:
			currentFrame = (int)Time.time % attackCursors.Length;
			activeCursor = attackCursors[currentFrame];
			break;
		case cursorState.Harvest:
			currentFrame = (int)Time.time % harvestCursors.Length;
			activeCursor = harvestCursors[currentFrame];
			break;
		case cursorState.Move:
			currentFrame = (int)Time.time % moveCursors.Length;
			activeCursor = moveCursors[currentFrame];
			break;
		case cursorState.PanLeft:
			activeCursor = leftCursor;
			break;
		case cursorState.PanRight:
			activeCursor = rightCursor;
			break;
		case cursorState.PanUp:
			activeCursor = upCursor;
			break;
		case cursorState.PanDown:
			activeCursor = downCursor;
			break;
		case cursorState.RallyPoint:
			activeCursor = rallyPointCursor;
			break;
		default: break;
		}
	}

	public void setResourceValues(Dictionary< resourceType, int > resourceValues, Dictionary< resourceType, int > resourceLimits)
	{
		this.resourceValues = resourceValues;
		this.resourceLimits = resourceLimits;
	}

	private int maxNumRows(int areaHeight)
	{
		return areaHeight / buildImageHeight;
	}

	private Rect getButtonPos(int row, int col)
	{
		int left = scrollBarWidth + col * buildImageWidth;
		float top = row * buildImageHeight - sliderValue * buildImageHeight;
		return new Rect (left, top, buildImageWidth, buildImageHeight);
	}

	private Rect getScrollPos(int groupHeight)
	{
		return new Rect (buttonSpacing, buttonSpacing, scrollBarWidth, groupHeight - 2 * buttonSpacing);
	}

	public cursorState getPreviousCursorState()
	{
		return previousCursorState;
	}

	public cursorState getCursorState()
	{
		return activeCursorState;
	}

}
