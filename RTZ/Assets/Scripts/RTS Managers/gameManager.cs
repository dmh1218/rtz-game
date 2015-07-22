using UnityEngine;
using System.Collections;
using RTS;

/*
 * Singleton that handles the management of game state. This includes
 * detecting when a game has been finished and what to do from there.
 */

public class gameManager : MonoBehaviour 
{
	//private variables
	private static bool created = false;
	private bool initialized = false;
	private VictoryConditions[] victoryConditions;
	private HUD hud;

	void Awake()
	{
		if (!created) {
			DontDestroyOnLoad (transform.gameObject);
			created = true;
			initialized = true;
		} else {
			Destroy (this.gameObject);
		}

		if (initialized) {
			loadDetails ();
		}
	}

	void OnLevelWasLoaded()
	{
		if (initialized) {
			loadDetails ();
		}
	}

	private void loadDetails()
	{
		Player[] players = GameObject.FindObjectsOfType (typeof(Player)) as Player[];
		foreach (Player player in players) {
			if (player.human) {
				hud = player.GetComponentInChildren<HUD> ();
			}
		}
		victoryConditions = GameObject.FindObjectsOfType(typeof(VictoryConditions)) as VictoryConditions[];
		if (victoryConditions != null) {
			foreach (VictoryConditions victoryCondition in victoryConditions) {
				victoryCondition.setPlayers (players);
			}
		}
	}

	void Update()
	{
		if (victoryConditions != null) {
			foreach (VictoryConditions victoryCondition in victoryConditions) {
				if (victoryCondition.gameFinished ()) {
					resultsScreen ResultsScreen = hud.GetComponent<resultsScreen> ();
					ResultsScreen.setMetVictoryCondition (victoryCondition);
					ResultsScreen.enabled = true;
					Time.timeScale = 0.0f;
					Cursor.visible = true;
					resourceManager.menuOpen = true;
					hud.enabled = false;
				}
			}
		}
	}

}
