using UnityEngine;
using System.Collections;
using RTS;

public class MainMenu : Menu 
{

	protected override void setButtons()
	{
		buttons = new string[] {"New Game", "Load Game", "Change Player", "Quit Game"};
	}

	protected override void handleButton(string text)
	{
		switch (text) {
		case "New Game":
			newGame ();
			break;
		case "Load Game":
			loadGame();
			break;
		case "Change Player":
			changePlayer();
			break;
		case "Quit Game":
			exitGame ();
			break;
		default:
			break;
		}
	}

	protected override void hideCurrentMenu()
	{
		GetComponent<MainMenu> ().enabled = false;
	}

	void onLevelWasLoaded()
	{
		Cursor.visible = true;
		if (playerManager.getPlayerName () == "") {
			//no player yet selected so enable setPlayerMenu
			GetComponent<MainMenu> ().enabled = false;
			GetComponent<SelectPlayerMenu> ().enabled = true;
		} else {
			//player selected so enable main menu
			GetComponent<MainMenu> ().enabled = true;
			GetComponent<SelectPlayerMenu> ().enabled = false;
		}
	}

	//private methods

	private void newGame()
	{
		resourceManager.menuOpen = false;
		Application.LoadLevel ("urban01");
		//run level at normal speed
		Time.timeScale = 1.0f;
	}

	private void changePlayer()
	{
		GetComponent<MainMenu> ().enabled = false;
		GetComponent<SelectPlayerMenu> ().enabled = true;
		selectionList.loadEntries (playerManager.getPlayerNames ());
	}


}
