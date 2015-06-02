using UnityEngine;
using RTS;

public class SelectPlayerMenu : MonoBehaviour 
{
	//public variables
	public GUISkin mySkin;

	//private variables
	private string playerName = "NewPlayer";

	void OnGUI()
	{
		GUI.skin = mySkin;

		float menuHeight = getMenuHeight ();
		float groupLeft = Screen.width / 2 - resourceManager.menuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		Rect groupRect = new Rect (groupLeft, groupTop, resourceManager.menuWidth, menuHeight);

		GUI.BeginGroup (groupRect);
		//background box
		GUI.Box (new Rect (0, 0, resourceManager.menuWidth, menuHeight), "");
		//menu buttons
		float leftPos = resourceManager.menuWidth / 2 - resourceManager.buttonWidth / 2;
		float topPos = menuHeight - resourceManager.padding - resourceManager.buttonHeight;
		if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), "Select")) {
			selectPlayer ();
		}
		//text area for player to type new name
		float textTop = menuHeight - 2 * resourceManager.padding - resourceManager.buttonHeight - resourceManager.textHeight;
		float textWidth = resourceManager.menuWidth - 2 * resourceManager.padding;

		playerName = GUI.TextField (new Rect (resourceManager.padding, textTop, textWidth, resourceManager.textHeight), playerName, 14);
		GUI.EndGroup ();
	}

	//private methods
	private float getMenuHeight()
	{
		return resourceManager.buttonHeight + resourceManager.textHeight + 3 * resourceManager.padding;
	}

	private void selectPlayer()
	{
		playerManager.selectPlayer (playerName);
		GetComponent<SelectPlayerMenu> ().enabled = false;
		MainMenu main = GetComponent<MainMenu> ();
		if (main) {
			main.enabled = true;
		}
	}

}
