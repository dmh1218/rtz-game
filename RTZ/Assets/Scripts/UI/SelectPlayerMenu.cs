using UnityEngine;
using RTS;

public class SelectPlayerMenu : MonoBehaviour 
{
	//public variables
	public GUISkin mySkin, selectionSkin;

	//private variables
	private string playerName = "NewPlayer";

	void Start()
	{
		playerManager.load ();
		selectionList.loadEntries (playerManager.getPlayerNames ());
	}

	void OnGUI()
	{
		if (selectionList.mouseDoubleClick ()) {
			playerName = selectionList.getCurrentEntry ();
			selectPlayer ();
		}

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
		selectionList.setCurrentEntry (playerName);
		GUI.EndGroup ();

		//selection list, needs to be called outside of the group for the menu
		float selectionLeft = groupRect.x + resourceManager.padding;
		float selectionTop = groupRect.y + resourceManager.padding;
		float selectionWidth = groupRect.width - 2 * resourceManager.padding;
		float selectionHeight = groupRect.height - getMenuItemsHeight () - resourceManager.padding;

		string prevSelection = selectionList.getCurrentEntry ();
		selectionList.Draw (selectionLeft, selectionTop, selectionWidth, selectionHeight, selectionSkin);
		string newSelection = selectionList.getCurrentEntry ();
		//set saveName to be the name selected in list if selection has changed
		if (prevSelection != newSelection) {
			playerName = newSelection;
		}
	}

	//private methods
	private float getMenuHeight()
	{
		return 250 + getMenuItemsHeight();
	}

	private float getMenuItemsHeight()
	{
//		float avatarHeight = 0;
//		if (avatars.Length > 0) {
//			avatarHeight = avatars [0].height + 2 * resourceManager.padding;
//		}
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
