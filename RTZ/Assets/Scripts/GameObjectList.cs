using UnityEngine;
using System.Collections;
using RTS;

public class GameObjectList : MonoBehaviour 
{
	public GameObject[] buildings;
	public GameObject[] units;
	public GameObject[] worldObjects;
	public GameObject player;

	private static bool created = false;

	void Awake()
	{
		if (!created) {
			DontDestroyOnLoad (transform.gameObject);
			resourceManager.setGameObjectList (this);
			created = true;
		} else {
			Destroy (this.gameObject);
		}
	}

	public GameObject getBuilding(string name)
	{
		for (int i = 0; i < buildings.Length; i++) {
			Building building = buildings [i].GetComponent<Building> ();
			if (building && building.name == name) {
				return buildings [i];
			}
		}
		return null;
	}

	public GameObject getUnit(string name)
	{
		for (int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent<Unit>();
			if (unit && unit.name == name) {
				return units[i];
			}
		}
		return null;
	}

	public GameObject getWorldObject(string name)
	{
		foreach (GameObject worldObject in worldObjects) {
			if (worldObject == null) {
				Debug.Log("worldobject null");
			}
			if (worldObject.name == name) {

				return worldObject;
			}
		}
		return null;
	}

	public GameObject getPlayerObject()
	{
		return player;
	}

	public Texture2D getBuildImage(string name)
	{
		for (int i = 0; i < buildings.Length; i++) {
			Building building = buildings [i].GetComponent<Building> ();
			if (building && building.name == name) {
				return building.buildImage;
			}
		}
		for (int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent<Unit>();
			if (unit && unit.name == name) {
				return unit.buildImage;
			}
		}
		return null;
	}

}
