using UnityEngine;
using RTS;

public class SaveMenu : MonoBehaviour 
{
	//public variables
	public GUISkin mySkin, selectionSkin;

	//private variables
	private string saveName = "NewGame";
	private confirmDialog confDialog = new confirmDialog();

	void Start()
	{
		activate ();
	}

	void Update()
	{
		//handle escape key
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (confDialog.isConfirming()) {
				confDialog.endConfirmation();
			} else {
				cancelSave ();
			}
		}

		//handle enter key in confirmation dialog
		if (Input.GetKeyDown (KeyCode.Return) && confDialog.isConfirming ()) {
			confDialog.endConfirmation ();
			saveGame ();
		}
	}

	void OnGUI()
	{
		if (confDialog.isConfirming ()) {
			string message = "\"" + saveName + "\" already exists. Do you wish to continue?";
			confDialog.show (message, mySkin);
		} else if (confDialog.madeChoice ()) {
			if (confDialog.clickedYes ()) {
				saveGame ();
			}
			confDialog.endConfirmation ();
		} else {
			if (selectionList.mouseDoubleClick ()) {
				saveName = selectionList.getCurrentEntry ();
				startSave ();
			}

			GUI.skin = mySkin;
			DrawMenu ();

			//handle enter being hit when typing in the text field
			if (Event.current.keyCode == KeyCode.Return) {
				startSave ();
			}
		}
	}

	//public methods
	public void activate()
	{
		selectionList.loadEntries (playerManager.getSavedGames ());

		if (resourceManager.levelName != null && resourceManager.levelName != "") {
			saveName = resourceManager.levelName;
		}
	}

	//private methods
	private void DrawMenu()
	{
		float menuHeight = getMenuHeight ();
		float groupLeft = Screen.width / 2 - resourceManager.menuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		Rect groupRect = new Rect (groupLeft, groupTop, resourceManager.menuWidth, menuHeight);

		GUI.BeginGroup (groupRect);

		//background box
		GUI.Box (new Rect (0, 0, resourceManager.menuWidth, menuHeight), "");

		//menu buttons
		float leftPos = resourceManager.padding;
		float topPos = menuHeight - resourceManager.padding - resourceManager.buttonHeight;

		if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), "Save Game")) {
			startSave ();
		}
		leftPos += resourceManager.buttonWidth + resourceManager.padding;
		if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), "Cancel")) {
			cancelSave ();
		}

		//text area for player to type new name
		float textTop = menuHeight - 2 * resourceManager.padding - resourceManager.buttonHeight - resourceManager.textHeight;
		float textWidth = resourceManager.menuWidth - 2 * resourceManager.padding;
		saveName = GUI.TextField (new Rect (resourceManager.padding, textTop, textWidth, resourceManager.textHeight), saveName, 60);
		selectionList.setCurrentEntry (saveName);
		GUI.EndGroup ();

		//selection list, needs to be called outside of the group for the menu
		string prevSelection = selectionList.getCurrentEntry ();
		float selectionLeft = groupRect.x + resourceManager.padding;
		float selectionTop = groupRect.y + resourceManager.padding;
		float selectionWidth = groupRect.width - 2 * resourceManager.padding;
		float selectionHeight = groupRect.height - getMenuItemsHeight () - resourceManager.padding;
		selectionList.Draw (selectionLeft, selectionTop, selectionWidth, selectionHeight, selectionSkin);
		string newSelection = selectionList.getCurrentEntry ();

		//set saveName to be the name selected in list if selection has changed
		if (prevSelection != newSelection) {
			saveName = newSelection;
		}
	}

	private float getMenuHeight()
	{
		return 250 + getMenuItemsHeight ();
	}

	private float getMenuItemsHeight()
	{
		return resourceManager.buttonHeight + resourceManager.textHeight + 3 * resourceManager.padding;
	}

	private void startSave()
	{
		//prompt for override of name if necessary
		if (selectionList.contains (saveName)) {
			confDialog.startConfirmation ();
		} else {
			saveGame ();
		}
	}

	private void cancelSave()
	{
		GetComponent<SaveMenu> ().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu> ();
		if (pause) {
			pause.enabled = true;
		}
	}

	private void saveGame()
	{
		saveManager.saveGame (saveName);
		resourceManager.levelName = saveName;

		GetComponent<SaveMenu> ().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu> ();
		if (pause) {
			pause.enabled = true;
		}
	}

}
