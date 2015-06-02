using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
	//public variables
	public float velocity = 1;
	public int damage = 1;

	//private variables
	private float range = 1;
	private WorldObject target;

	void Update()
	{
		if (hitSomething ()) {
			inflictDamage ();
			Destroy (gameObject);
		}

		if (range > 0) {
			float positionChange = Time.deltaTime * velocity;
			range -= positionChange;
			transform.position += (positionChange * transform.forward);
		} else {
			//causing error when below is uncommented
			//Destroy (gameObject);
		}
	}

	public void setRange(float range)
	{
		this.range = range;
	}

	public void setTarget(WorldObject target)
	{
		this.target = target;
	}

	private bool hitSomething()
	{
		if (target && target.getSelectionBounds ().Contains (transform.position)) {
			return true;
		}
		return false;
	}

	private void inflictDamage()
	{
		if (target) {
			target.takeDamage (damage);
		}
	}


}
