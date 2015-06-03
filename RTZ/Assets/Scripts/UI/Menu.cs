using UnityEngine;
using System.Collections;
using RTS;

public class Menu : MonoBehaviour 
{
	//public variables
	public GUISkin mySkin;
	public Texture2D header;
	

	//protected variables
	protected string[] buttons;

	protected virtual void Start()
	{
		setButtons();
	}

	protected virtual void OnGUI()
	{
		DrawMenu();
	}

	protected virtual void DrawMenu()
	{
		//default implementation for a menu consisting of a vertical list of buttons
		GUI.skin = mySkin;
		float menuHeight = getMenuHeight ();

		float groupLeft = Screen.width / 2 - resourceManager.menuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		GUI.BeginGroup (new Rect (groupLeft, groupTop, resourceManager.menuWidth, menuHeight));

		//background box
		GUI.Box (new Rect (0, 0, resourceManager.menuWidth, menuHeight), "");
		//header image
		GUI.DrawTexture (new Rect (resourceManager.padding, resourceManager.padding, resourceManager.headerWidth, resourceManager.headerHeight), header);

		//welcome message
		float leftPos = resourceManager.padding;
		float topPos = 2 * resourceManager.padding + header.height;
		GUI.Label (new Rect (leftPos, topPos, resourceManager.menuWidth - 2 * resourceManager.padding, resourceManager.textHeight), "Welcome " + playerManager.getPlayerName ());

		//menu buttons
		if (buttons != null) {
			leftPos = resourceManager.menuWidth / 2 - resourceManager.buttonWidth / 2;
			topPos += resourceManager.padding + resourceManager.textHeight;

			for (int i = 0; i < buttons.Length; i++) {
				if (i > 0) {
					topPos += resourceManager.buttonHeight + resourceManager.padding;
				}
				if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), buttons [i])) {
					handleButton (buttons [i]);
				}
			}
		}

		GUI.EndGroup ();
	}

	protected virtual void setButtons()
	{
		//a child class needs to set this for buttons to appear
	}

	protected virtual void handleButton(string text)
	{
		//a child class needs to set this to handle button clicks
	}

	protected virtual float getMenuHeight()
	{
		float buttonHeight = 0;
		float paddingHeight = 2 * resourceManager.padding;
		float messageHeight = resourceManager.textHeight + resourceManager.padding;

		if (buttons != null) {
			buttonHeight = buttons.Length * resourceManager.buttonHeight;
		}

		if (buttons != null) {
			paddingHeight += buttons.Length * resourceManager.padding;
		}

		return resourceManager.headerHeight + buttonHeight + paddingHeight + messageHeight;
	}

	protected void exitGame()
	{
		Application.Quit ();
	}
}
