﻿using UnityEngine;
using System.Collections;
using RTS;
using Newtonsoft.Json;

public class Tank : Unit 
{
	//private variables
	private Quaternion aimRotation;

	protected override void Start () 
	{
		base.Start ();
	}

	protected override void Update () 
	{
		base.Update ();
		if (aiming) {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, aimRotation, weaponAimSpeed);
			calculateBounds ();
			Quaternion inverseAimRotation = new Quaternion (-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
			if (transform.rotation == aimRotation || transform.rotation == inverseAimRotation) {
				aiming = false;
			}
		}
	}

	protected override void aimAtTarget()
	{
		base.aimAtTarget ();
		aimRotation = Quaternion.LookRotation (target.transform.position - transform.position);
	}

	protected override void useWeapon()
	{
		base.useWeapon ();
		Vector3 spawnPoint = transform.position;
		spawnPoint.x += (2.1f * transform.forward.x);
		spawnPoint.y += 1.4f;
		spawnPoint.z += (2.1f * transform.forward.z);
		GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("TankProjectile"), spawnPoint, transform.rotation);
		Projectile projectile = gameObject.GetComponentInChildren<Projectile> ();
		projectile.setRange (0.9f * weaponRange);
		projectile.setTarget (target);
	}

	protected override void handleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
	{
		base.handleLoadedProperty (reader, propertyName, readValue);

		switch (propertyName) {
		case "AimRotation":
			aimRotation = loadManager.loadQuaternion(reader);
			break;
		default:
			break;
		}
	}

	//save aspects unique to tanks
	public override void saveDetails(JsonWriter writer)
	{
		base.saveDetails (writer);

		saveManager.writeQuaternion (writer, "AimRotation", aimRotation);
	}

	public override bool canAttack()
	{
		return true;
	}
}
