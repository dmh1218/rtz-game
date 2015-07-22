using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour 
{
	//public variables
	public int damage = 1;

	//private variables
	//private float range = 0;
	private WorldObject target;

	void Update()
	{
		if (hitSomething ()) {
			inflictDamage ();
			Destroy (gameObject);
		}
	}

	//public methods

	public void setTarget(WorldObject target)
	{
		this.target = target;
	}

	//private methods
	
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
