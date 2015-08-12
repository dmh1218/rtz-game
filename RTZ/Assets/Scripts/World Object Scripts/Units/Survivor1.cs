using UnityEngine;
using System.Collections;
using RTS;
using Newtonsoft.Json;

public class Survivor1 : Unit 
{

	//public variables
	public GameObject currentGear;
	public GameObject[] possibleGear;

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
		GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("Bullet"), spawnPoint, transform.rotation);
		Projectile projectile = gameObject.GetComponentInChildren<Projectile> ();





//		Vector3 targetExtents = target.GetComponent<Renderer> ().bounds.extents;
//		Vector3 destination;
//		//float distance = Vector3.Distance (targetExtents, spawnPoint);
//		 
//
//		Vector3 direction = new Vector3 (targetExtents.x - spawnPoint.x, 0.0f, targetExtents.z - spawnPoint.z);
//		direction.Normalize ();
//
//		for (int i = 0; i < shiftAmount; i++) {
//			destination -= direction;
//		}
//
//		projectile.setRange (range + weaponRange);






		projectile.setRange (0.9f * weaponRange);
		projectile.setTarget (target);
	}

	public override bool canAttack()
	{
		return true;
	}
}
