using UnityEngine;
using System.Collections;
using RTS;
using Newtonsoft.Json;

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

		resType = resourceType.Unknown;
		if (loadedSavedValues) {
			return;
		}
		amountLeft = capacity;
	}

	protected override void calculateCurrentHealth(float lowSplit, float highSplit)
	{
		healthPercentage = amountLeft / capacity;
		healthStyle.normal.background = resourceManager.getResourceHealthBar (resType);
	}

	protected override void handleLoadedProperty (JsonTextReader reader, string propertyName, object readValue)
	{
		base.handleLoadedProperty (reader, propertyName, readValue);
		switch (propertyName) {
		case "AmountLeft": 
			amountLeft = (float)loadManager.convertToFloat(readValue);
			break;
		default:
			break;
		}
	}

	protected override bool shouldMakeDecision()
	{
		return false;
	}

	//override function to save details unique to expendable resources
	public override void saveDetails(JsonWriter writer)
	{
		base.saveDetails (writer);
		saveManager.writeFloat (writer, "AmountLeft", amountLeft);
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
