using UnityEngine;
using System.Collections;
using RTS;

public class HUD : MonoBehaviour 
{
	public GUISkin ordersSkin;
	public GUISkin resourceSkin;
	public GUISkin selectBoxSkin;
	public GUISkin mouseCursorSkin;
	public Texture2D activeCursor;
	public Texture2D selectCursor;
	public Texture2D leftCursor;
	public Texture2D rightCursor;
	public Texture2D upCursor;
	public Texture2D downCursor;
	public Texture2D[] moveCursors;
	public Texture2D[] attackCursors;
	public Texture2D[] harvestCursors;

	private Player player;
	private static int ordersBarWidth = 150;
	private static int resourceBarHeight = 40;
	private static int selectionNameHeight = 15;
	private cursorState activeCursorState;
	private int currentFrame = 0;

	Vector3 mousePos;
	bool insideWidth;
	bool insideHeight;

	string selectionName;

	void Start()
	{
		player = transform.root.GetComponent<Player> ();
		resourceManager.storeSelectBoxItems (selectBoxSkin);
		setCursorState (cursorState.Select);
	}

	void OnGUI()
	{
		if (player && player.human) {
			DrawOrdersBar();
			DrawResourceBar();
			DrawMouseCursor();
		}
	}

	private void DrawOrdersBar()
	{
		GUI.skin = ordersSkin;
		GUI.BeginGroup (new Rect (Screen.width - ordersBarWidth, resourceBarHeight, ordersBarWidth, Screen.height - resourceBarHeight));
		GUI.Box (new Rect (0, 0, ordersBarWidth, Screen.height - resourceBarHeight), "");

		selectionName = "";
		if (player.selectedObject) {
			selectionName = player.selectedObject.objectName;
		}
		if (!selectionName.Equals ("")) {
			GUI.Label (new Rect (0, 10, ordersBarWidth, selectionNameHeight), selectionName);
		}
		GUI.EndGroup ();
	}

	private void DrawResourceBar()
	{
		GUI.skin = resourceSkin;
		GUI.BeginGroup (new Rect (0, 0, Screen.width, resourceBarHeight));
		GUI.Box (new Rect (0, 0, Screen.width, resourceBarHeight), "");
		GUI.EndGroup ();
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

	private void DrawMouseCursor()
	{
		bool mouseOverHUD = !mouseInBounds() && activeCursorState != cursorState.PanRight && activeCursorState != cursorState.PanUp;

		if (mouseOverHUD) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;
			GUI.skin = mouseCursorSkin;
			GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
			updateCursorAnimation();
			Rect cursorPosition = getCursorDrawPosition();
			GUI.Label(cursorPosition, activeCursor);
			GUI.EndGroup();
		}
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
		}
		return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
	}

	public void setCursorState(cursorState newState)
	{
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
		default: break;
		}
	}

}
