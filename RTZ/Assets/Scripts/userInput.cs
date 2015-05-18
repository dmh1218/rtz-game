using UnityEngine;
using System.Collections;
using RTS;

public class userInput : MonoBehaviour 
{
	private Player player;

	float xpos;
	float ypos;
	Vector3 origin;
	Vector3 destination;
	Vector3 oldRotation;
	Vector3 newRotation;

	Vector3 initPosition;
	Vector3 initRotation;
	float currZoom = 0;
	Vector2 zoomAngleRange = new Vector2( 20, 70 );
	Vector2 zoomRange = new Vector2 (-10, 100);

	void Start()
	{
		player = transform.root.GetComponent<Player> ();
	}

	void Update()
	{
		if (player.human) {
			moveCamera();
			rotateCamera();
		}
	}

	void moveCamera()
	{
		xpos = Input.mousePosition.x;
		ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3 (0, 0, 0);

		//horizontal camera movement
		if ((xpos >= 0 && xpos < resourceManager.scrollwidth) || ((Input.GetKey (KeyCode.A)))) {
			movement.x -= resourceManager.scrollSpeed;
		} else if ((xpos <= Screen.width && xpos > Screen.width - resourceManager.scrollwidth) || ((Input.GetKey (KeyCode.D)))) {
			movement.x += resourceManager.scrollSpeed;
		}

		//vertical camera movement
		if ((ypos >= 0 && ypos < resourceManager.scrollwidth) || ((Input.GetKey (KeyCode.S)))) {
			movement.z -= resourceManager.scrollSpeed;
		} else if ((ypos <= Screen.height && ypos > Screen.height - resourceManager.scrollwidth) || ((Input.GetKey (KeyCode.W)))) {
			movement.z += resourceManager.scrollSpeed;
		}

		//make sure movement is in the direction the camera is pointing 
		//but ignore the vertical tilt of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection (movement);
		movement.y = 0;

		//away from ground movement
		movement.y -= resourceManager.scrollSpeed * Input.GetAxis ("Mouse ScrollWheel");

		//calculate desired camera position based on received input
		origin = Camera.main.transform.position;
		destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		//limit away from ground movement to be between a minimum and a maximum distance
		if (destination.y > resourceManager.MaxCameraHeight) {
			destination.y = resourceManager.MaxCameraHeight;
		} else if (destination.y < resourceManager.minCameraHeight) {
			destination.y = resourceManager.minCameraHeight;
		}

		//if a change in position is detected, perform the necessary update
		if (destination != origin) {
			Camera.main.transform.position = Vector3.MoveTowards (origin, destination, Time.deltaTime * resourceManager.scrollSpeed);
		}
	}

	void rotateCamera()
	{
		//rotation method I wrote
		if ((Input.GetKey (KeyCode.Q))) {
			Camera.main.transform.Rotate (Vector3.up * Time.deltaTime * -resourceManager.rotateSpeed, Space.World);
		} else if ((Input.GetKey (KeyCode.E))) {
			Camera.main.transform.Rotate (Vector3.up * Time.deltaTime * resourceManager.rotateSpeed, Space.World);
		}


		//rotation method from tutorial
//		oldRotation = Camera.main.transform.eulerAngles;
//		newRotation = oldRotation;
//
//		//detect rotation amount if ALT is being held and the right mouse button is down
//		if ((Input.GetKey(KeyCode.LeftAlt) || (Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))) {
//			newRotation.x -= Input.GetAxis("Mouse Y") * resourceManager.rotateAmount;
//			newRotation.y += Input.GetAxis("Mouse X") * resourceManager.rotateAmount;
//		}
//
//		//if a change in position is detected, perform the necessary update
//		if (newRotation != oldRotation) {
//			Camera.main.transform.eulerAngles = Vector3.MoveTowards(oldRotation, newRotation, Time.deltaTime * resourceManager.rotateSpeed);
//		}
	}

	void zoomCamera()
	{
		initPosition = Camera.main.transform.position;
		initRotation = Camera.main.transform.eulerAngles;

		//ZOOM IN/OUT
		currZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * resourceManager.zoomSpeed;
		currZoom = Mathf.Clamp(currZoom, zoomRange.x, zoomRange.y);
		
		transform.position = new Vector3 (transform.position.x, transform.position.y - (transform.position.y - (initPosition.y + currZoom)) * 0.1f, transform.position.z);
		
		float x = transform.eulerAngles.x - (transform.eulerAngles.x - (initRotation.x + currZoom * resourceManager.zoomRotation)) * 0.1f;
		x = Mathf.Clamp (x, zoomAngleRange.x, zoomAngleRange.y);
		
		transform.eulerAngles = new Vector3 (x, transform.eulerAngles.y, transform.eulerAngles.z);
	}

}
