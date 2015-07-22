using UnityEngine;
using RTS;

/*
 * Singleton that handles loading level details. This includes making sure
 * that all world objects have an objectId set
 */

public class LevelLoader : MonoBehaviour 
{
	//private variables
	private static int nextObjectId = 0;
	private static bool created = false;
	private bool initialized = false;

	void Awake()
	{
		if (initialized) {
			SelectPlayerMenu menu = GameObject.FindObjectOfType (typeof(SelectPlayerMenu)) as SelectPlayerMenu;
			if (!menu) {
				//we have started from inside a map, rather than the main menu
				//this happens if we launch Unity from inside a map for testing
				Player[] players = GameObject.FindObjectsOfType(typeof(Player)) as Player[];
				foreach (Player player in players) {
					if (player.human) {
						playerManager.selectPlayer(player.username);
					}
				}
				setObjectIds ();
			}
		}

		if (!created) {
			DontDestroyOnLoad (transform.gameObject);
			created = true;
			initialized = true;
		} else {
			Destroy (this.gameObject);
		}
	}

	void OnLevelWasLoaded()
	{
		if (initialized) {
			if (resourceManager.levelName != null && resourceManager.levelName != "") {
				loadManager.loadGame(resourceManager.levelName);
			} else {
				setObjectIds();
			}

			Time.timeScale = 1.0f;
			resourceManager.menuOpen = false;
		}
	}

	//public methods

	public int getNewObjectId()
	{
		nextObjectId++;
		if (nextObjectId >= int.MaxValue) {
			nextObjectId = 0;
		}

		return nextObjectId;
	}

	//private methods

	private void setObjectIds()
	{
		WorldObject[] worldObjects = GameObject.FindObjectsOfType (typeof(WorldObject)) as WorldObject[];

		foreach (WorldObject worldObject in worldObjects) {
			worldObject.objectId = nextObjectId++;
			if (nextObjectId >= int.MaxValue) {
				nextObjectId = 0;
			}
		}
	}
}
