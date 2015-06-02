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
		buttons = new string[] {"Resume", "Exit Game"};
	}

	protected override void handleButton (string text)
	{
		switch (text) {
		case "Resume":
			resume ();
			break;
		case "Exit Game":
			returnToMainMenu ();
			break;
		default:
			break;
		}
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

	private void returnToMainMenu()
	{
		Application.LoadLevel ("MainMenu");
		Cursor.visible = true;
	}
}
