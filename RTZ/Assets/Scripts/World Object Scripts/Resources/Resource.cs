using UnityEngine;
using System.Collections;
using RTS;

public class Resource : WorldObject 
{
	//Public Variables
	public float capacity;

	//Variables accessible by subclass
	protected float amountLeft;
	protected resourceType resType;

	/*** Game Engine methods, all can be overriden by subclass ***/

	protected override void Start()
	{
		base.Start ();

		amountLeft = capacity;
		resType = resourceType.Unknown;
	}

	protected override void calculateCurrentHealth(float lowSplit, float highSplit)
	{
		healthPercentage = amountLeft / capacity;
		healthStyle.normal.background = resourceManager.getResourceHealthBar (resType);
	}

	/*** Public methods ***/

	public void remove(float amount)
	{
		amountLeft -= amount;
		if (amountLeft < 0) {
			amountLeft = 0;
		}
	}

	public bool isEmpty()
	{
		return amountLeft <= 0;
	}

	public resourceType getResourceType()
	{
		return resType;
	}

}
