using UnityEngine;
using RTS;

public class FoodCrate : Resource 
{

	protected override void Start()
	{
		base.Start ();
		resType = resourceType.Food;
	}
	
	protected override void Update()
	{
		base.Update ();

		float percentLeft = (float)amountLeft / (float)capacity;
		if (percentLeft < 0) {
			percentLeft = 0;
		}

		calculateBounds ();
	}
	
}
