using UnityEngine;
using RTS;

public class PauseMenu : Menu 
{
	//private variables
	private Player player;

	protected override void Start()
	{
		base.Start ();
		player = transform.root.GetComponent<Player> ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			resume ();
		}
	}

	protected override void setButtons()
	{
		buttons = new string[] {"Resume", "Save Game", "Load Game", "Exit Game"};
	}

	protected override void handleButton (string text)
	{
		switch (text) {
		case "Resume":
			resume ();
			break;
		case "Save Game":
			saveGame();
			break;
		case "Load Game":
			loadGame();
			break;
		case "Exit Game":
			returnToMainMenu ();
			break;
		default:
			break;
		}
	}

	protected override void hideCurrentMenu()
	{
		GetComponent<PauseMenu> ().enabled = false;
	}

	/*** Private methods ***/
	private void resume()
	{
		Time.timeScale = 1.0f;
		GetComponent<PauseMenu> ().enabled = false;
		if (player) {
			player.GetComponent<userInput> ().enabled = true;
		}
		Cursor.visible = false;
		resourceManager.menuOpen = false;
	}

	private void saveGame()
	{
		GetComponent<PauseMenu> ().enabled = false;
		SaveMenu saveMenu = GetComponent<SaveMenu> ();
		if (saveMenu) {
			saveMenu.enabled = true;
			saveMenu.activate ();
		}
	}

	private void returnToMainMenu()
	{
		resourceManager.levelName = "";
		Application.LoadLevel ("MainMenu");
		Cursor.visible = true;
	}
}
