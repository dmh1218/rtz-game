using UnityEngine;
using System.Collections;
using RTS;

public class MainMenu : Menu 
{

	protected override void setButtons()
	{
		buttons = new string[] {"New Game", "Quit Game"};
	}

	protected override void handleButton(string text)
	{
		switch (text) {
		case "New Game":
			newGame ();
			break;
		case "Quit Game":
			exitGame ();
			break;
		default:
			break;
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


}
