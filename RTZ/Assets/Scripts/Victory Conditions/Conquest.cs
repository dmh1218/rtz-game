using UnityEngine;
using System.Collections;

public class Conquest : VictoryConditions 
{
	//override methods

	public override string getDescription()
	{
		return "Conquest";
	}

	public override bool gameFinished()
	{
		if (players == null) {
			return true;
		}

		int playersLeft = players.Length;

		foreach (Player player in players) {
			if (!playerMeetsConditions (player)) {
				playersLeft--;
			}
		}

		return playersLeft == 1;
	}

	public override bool playerMeetsConditions(Player player)
	{
		return player && !player.isDead ();
	}
}
