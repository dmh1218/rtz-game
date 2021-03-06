﻿using UnityEngine;
using System.Collections;
using RTS;
using Newtonsoft.Json;

public class Zombie1 : Unit
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
		spawnPoint.x += (1f * transform.forward.x);
		spawnPoint.y += 2f;
		spawnPoint.z += (2.1f * transform.forward.z);
		GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("ZombieAttack"), spawnPoint, transform.rotation);
		MeleeAttack meleeAttack = gameObject.GetComponentInChildren<MeleeAttack> ();
		meleeAttack.setTarget (target);

		//later implement melee attack animation
	}

	public override bool canAttack()
	{
		return true;
	}
}
