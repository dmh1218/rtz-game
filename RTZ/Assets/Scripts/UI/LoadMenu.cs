using UnityEngine;
using RTS;

public class LoadMenu : MonoBehaviour 
{
	//public variables
	public GUISkin mainSkin, selectionSkin;

	void Start()
	{
		activate ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			cancelLoad ();
		}
	}

	void OnGUI()
	{
		if (selectionList.mouseDoubleClick ()) {
			startLoad ();
		}

		GUI.skin = mainSkin;
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

		if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), "Load Game")) {
			startLoad();
		}
		leftPos += resourceManager.buttonWidth + resourceManager.padding;
		if (GUI.Button (new Rect (leftPos, topPos, resourceManager.buttonWidth, resourceManager.buttonHeight), "Cancel")) {
			cancelLoad();
		}
		GUI.EndGroup ();

		//selection list, needs to be called outside of the group for the menu
		float selectionLeft = groupRect.x + resourceManager.padding;
		float selectionTop = groupRect.y + resourceManager.padding;
		float selectionWidth = groupRect.width - 2 * resourceManager.padding;
		float selectionHeight = groupRect.height - getMenuItemsHeight () - resourceManager.padding;

		selectionList.Draw (selectionLeft, selectionTop, selectionWidth, selectionHeight, selectionSkin);
	}

	//private methods for load Menu

	private float getMenuHeight()
	{
		return 250 + getMenuItemsHeight ();
	}

	private float getMenuItemsHeight()
	{
		return resourceManager.buttonHeight + 2 * resourceManager.padding;
	}

	private void startLoad()
	{
		string newlevel = selectionList.getCurrentEntry ();
		
		if (newlevel != "") {
			resourceManager.levelName = newlevel;
			if (Application.loadedLevelName != "BlankMap1") {
				Application.LoadLevel("BlankMap1");
			} else if (Application.loadedLevelName != "BlankMap2") {
				Application.LoadLevel("BlankMap2");
			}
			//makes sure that the loaded level runs at normal speed
			Time.timeScale = 1.0f;
		}
	}

	private void cancelLoad()
	{
		GetComponent<LoadMenu> ().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu> ();
		if (pause) {
			pause.enabled = true;
		} else {
			MainMenu main = GetComponent<MainMenu> ();
			if (main) {
				main.enabled = true;
			}
		}
	}

	//public methods for load menu

	public void activate()
	{
		selectionList.loadEntries (playerManager.getSavedGames ());
	}



}
