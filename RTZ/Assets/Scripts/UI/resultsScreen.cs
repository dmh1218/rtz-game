using UnityEngine;
using System.Collections.Generic;
using RTS;

public class resultsScreen : MonoBehaviour 
{
	//public variables
	public GUISkin skin;
	//public AudioClip clickSound;
	//public float clickVolume = 1.0f;

	//private variables
	private Player winner;
	private VictoryConditions metVictoryCondition;
	//private AudioElement audioElement;

	void Start()
	{
		//audio stuff
	}

	void OnGUI()
	{
		GUI.skin = skin;
		GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));

		//display
		float padding = resourceManager.padding;
		float itemHeight = resourceManager.buttonHeight;
		float buttonWidth = resourceManager.buttonWidth;
		float leftPos = Screen.width / 2 - Screen.width / 6;
		float topPos = Screen.height / 2 - Screen.height / 6;
		//GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "");

		GUI.Box (new Rect (leftPos, topPos, Screen.width / 3, Screen.height / 3), "");
		string message = "Game Over";

		if (winner) {
			message = "Congratulations " + winner.username + "! You have won by " + metVictoryCondition.getDescription ();
		}

		//GUI.Label (new Rect (leftPos, topPos, Screen.width - 2 * padding, itemHeight), message);

		topPos += 4 * padding;
		leftPos += 4 * padding;

		GUI.Label (new Rect (leftPos, topPos, Screen.width - 2 * padding, itemHeight), message);

//		if (GUI.Button (new Rect (leftPos, topPos, buttonWidth, itemHeight), "New Game")) {
//			playClick();

			//makes sure that the loaded level runs at normal speed
//			Time.timeScale = 1.0f;
//			resourceManager.menuOpen = false;
//			Application.LoadLevel ("urban01");
//		}

		leftPos = Screen.width / 2 - buttonWidth / 2;
		topPos += itemHeight + padding;

		if (GUI.Button (new Rect (leftPos, topPos, buttonWidth, itemHeight), "Main Menu")) {
			resourceManager.levelName = "";
			Application.LoadLevel ("MainMenu");
			Cursor.visible = true;
		}

		GUI.EndGroup ();
	}

	//private methods

	private void playClick()
	{
//		if (audioElement != null) {
//			audioElement.Play (clickSound);
//		}
	}

	//public methods

	public void setMetVictoryCondition(VictoryConditions victoryCondition)
	{
		if (!victoryCondition) {
			return;
		}

		metVictoryCondition = victoryCondition;
		winner = metVictoryCondition.getWinner();
	}
}
