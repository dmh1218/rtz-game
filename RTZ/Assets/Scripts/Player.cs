using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject selectedObject { get; set; }

	void Start()
	{
		hud = GetComponentInChildren<HUD> ();
	}
	
}

