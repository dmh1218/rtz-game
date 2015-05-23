using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour 
{
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject selectedObject { get; set; }
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;

	private Dictionary<resourceType, int> resources, resourceLimits;

	void Start()
	{
		hud = GetComponentInChildren<HUD> ();
		addStartResourceLimits ();
		addStartResources ();
	}

	void Awake()
	{
		resources = initResourceList ();
		resourceLimits = initResourceList ();
	}

	void Update()
	{
		if (human) {
			hud.setResourceValues (resources, resourceLimits);
		}
	}

	private Dictionary< resourceType, int > initResourceList()
	{
		Dictionary< resourceType, int > list = new Dictionary< resourceType, int > ();
		list.Add (resourceType.Money, 0);
		list.Add (resourceType.Power, 0);
		return list;
	}

	private void addStartResourceLimits()
	{
		incrementResourceLimit (resourceType.Money, startMoneyLimit);
		incrementResourceLimit (resourceType.Power, startPowerLimit);
	}

	private void addStartResources()
	{
		addResource (resourceType.Money, startMoney);
		addResource (resourceType.Power, startPower);
	}

	public void addResource (resourceType type, int amount)
	{
		resources [type] += amount;
	}

	public void incrementResourceLimit (resourceType type, int amount)
	{
		resourceLimits [type] += amount;
	}

	public void addUnit (string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator)
	{
		Debug.Log ("add" + unitName + " to player");
		Units units = GetComponentInChildren<Units> ();
		GameObject newUnit = (GameObject)Instantiate (resourceManager.getUnit (unitName), spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
		Unit unitObject = newUnit.GetComponent<Unit> ();
		if (unitObject && spawnPoint != rallyPoint) {
			unitObject.startMove (rallyPoint);
		}
		if (unitObject) {
			unitObject.init (creator);
			if (spawnPoint != rallyPoint) {
				unitObject.startMove (rallyPoint);
			}
		}
	}
}

