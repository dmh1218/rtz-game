using UnityEngine;

public class Refinery : Building 
{
	protected override void Start()
	{
		base.Start ();
		actions = new string[] {"Harvester"};
	}

	protected override bool shouldMakeDecision()
	{
		return false;
	}

	public override void PerformAction(string actionToPerform)
	{
		base.PerformAction (actionToPerform);
		createUnit (actionToPerform);
	}
}
