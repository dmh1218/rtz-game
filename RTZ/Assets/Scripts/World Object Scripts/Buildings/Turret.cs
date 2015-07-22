using UnityEngine;
using System.Collections;
using RTS;

public class Turret : Building 
{
	//private variables

	private Quaternion aimRotation;

	protected override void Start()
	{
		base.Start ();
		detectionRange = weaponRange;
	}

	protected override void Update()
	{
		base.Update ();

		if (aiming) {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, aimRotation, weaponAimSpeed);
			calculateBounds();
			//sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
			Quaternion inverseAimRotation = new Quaternion(-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
			if (transform.rotation == aimRotation || transform.rotation == inverseAimRotation) {
				aiming = false;
			}
		}
	}

	protected override void useWeapon()
	{
		base.useWeapon ();

		Vector3 spawnPoint = transform.position;
		spawnPoint.x += (2.6f * transform.forward.x);
		spawnPoint.y += 1.0f;
		spawnPoint.z += (2.6f * transform.forward.z);
		GameObject gameObject = (GameObject)Instantiate (resourceManager.getWorldObject ("TurretProjectile"), spawnPoint, transform.rotation);
		Projectile projectile = gameObject.GetComponentInChildren<Projectile> ();
		projectile.setRange (0.9f * weaponRange);
		projectile.setTarget (target);
	}

	protected override void aimAtTarget()
	{
		base.aimAtTarget ();
		aimRotation = Quaternion.LookRotation (target.transform.position - transform.position);
	}

	public override bool canAttack()
	{
		if (underConstruction () || hitPoints == 0) {
			return false;
		}

		return true;
	}

}
