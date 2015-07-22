using UnityEngine;
using System.Collections;

public class RallyPoint : MonoBehaviour 
{
	public void enable()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = true;
		}
	}

	public void disable()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = false;
		}
	}

}
