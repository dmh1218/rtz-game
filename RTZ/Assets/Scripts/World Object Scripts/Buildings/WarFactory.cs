﻿using UnityEngine;
using System.Collections;

public class WarFactory : Building 
{
	protected override void Start ()
	{
		base.Start ();
		actions = new string[] { "Tank" };
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
