using UnityEngine;
using System.Collections;

public abstract class VictoryConditions : MonoBehaviour 
{
	//protected variables
	protected Player[] players;

	public void setPlayers(Player[] players)
	{
		this.players = players;
	}

	public Player[] getPlayers()
	{
		return players;
	}

	public virtual bool gameFinished()
	{
		if (players == null) {
			return true;
		}

		foreach (Player player in players) {
			if (playerMeetsConditions (player)) {
				return true;
			}
		}

		return false;
	}

	public Player getWinner()
	{
		if (players == null) {
			return null;
		}

		foreach (Player player in players) {
			if (playerMeetsConditions(player)) {
				return player;
			}
		}

		return null;
	}

	public abstract string getDescription();

	public abstract bool playerMeetsConditions(Player player);

}
