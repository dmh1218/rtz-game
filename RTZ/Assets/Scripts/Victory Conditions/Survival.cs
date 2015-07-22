using UnityEngine;
using System.Collections;

public class Survival : VictoryConditions 
{
	//public variables

	public int timeLimit = 1;

	//private variables

	private float timeLeft = 0.0f;

	//built in methods

	void Awake()
	{
		timeLeft = timeLimit * 60;
	}

	void Update()
	{
		timeLeft -= Time.deltaTime;
	}
	
	//override methods
	public override string getDescription()
	{
		return "Survival";
	}

	public override bool gameFinished()
	{
		foreach (Player player in players) {
			if (player && player.human && player.isDead ()) {
				return true;
			}
		}
		return timeLeft < 0;
	}

	public override bool playerMeetsConditions (Player player)
	{
		return player && player.human && !player.isDead ();
	}
}
