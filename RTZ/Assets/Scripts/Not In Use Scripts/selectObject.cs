using UnityEngine;
using System.Collections;

public class selectObject : MonoBehaviour 
{

	bool selected;

	void Start()
	{
		selected = false;
	}

	void Update()
	{
		if (selected) {
			//if left click on other object, call deselect=
		}
	}

	void select()
	{
		selected = true;
		//show UI ring around object
		//show selected object in HUD
		//
	}

	void deselect()
	{
		//hide UI ring around object
		//hide selected object in HUD
		//
	}

	public bool isSelected()
	{
		return selected;
	}
}
