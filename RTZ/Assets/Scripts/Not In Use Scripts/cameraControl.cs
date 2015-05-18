using UnityEngine;
using System.Collections;

public class cameraControl : MonoBehaviour 
{

	public float scrollSpeed = 15f;
	public float scrollEdge = 0.01f;
	public float panSpeed = 10f;
	public Vector2 zoomRange = new Vector2 (-10, 100);
	public float currZoom = 0f;
	public float zoomSpeed = 1f;
	public float zoomRotation = 1f;
	public Vector2 zoomAngleRange = new Vector2( 20, 70 );
	public float rotateSpeed = 10f;

	Vector3 initPosition;
	Vector3 initRotation;

	void Start()
	{
		initPosition = transform.position;
		initRotation = transform.eulerAngles;
	}

	void Update()
	{
		//PAN
		if ((Input.GetKey (KeyCode.D)) || (Input.mousePosition.x >= Screen.width * (1 - scrollEdge))) {
			transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
		} else if ((Input.GetKey (KeyCode.A)) || (Input.mousePosition.x <= Screen.width * scrollEdge)) {
			transform.Translate(Vector3.right * Time.deltaTime * -scrollSpeed, Space.World);
		} else if ((Input.GetKey (KeyCode.W)) || (Input.mousePosition.y >= Screen.height * (1 - scrollEdge))) {
			transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);
		} else if ((Input.GetKey (KeyCode.S)) || (Input.mousePosition.y <= Screen.width * scrollEdge)) {
			transform.Translate(Vector3.forward * Time.deltaTime * -scrollSpeed, Space.World);
		}

		//ROTATE
		if ((Input.GetKey (KeyCode.Q))) {
			transform.Rotate (Vector3.up * Time.deltaTime * -rotateSpeed, Space.World);
		} else if ((Input.GetKey (KeyCode.E))) {
			transform.Rotate (Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
		}

		//ZOOM IN/OUT
		currZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * zoomSpeed;
		currZoom = Mathf.Clamp(currZoom, zoomRange.x, zoomRange.y);

		transform.position = new Vector3 (transform.position.x, transform.position.y - (transform.position.y - (initPosition.y + currZoom)) * 0.1f, transform.position.z);

		float x = transform.eulerAngles.x - (transform.eulerAngles.x - (initRotation.x + currZoom * zoomRotation)) * 0.1f;
		x = Mathf.Clamp (x, zoomAngleRange.x, zoomAngleRange.y);

		transform.eulerAngles = new Vector3 (x, transform.eulerAngles.y, transform.eulerAngles.z);

//		transform.position.y -= (transform.position.y - (initPosition.y + currZoom)) * 0.1;
//		transform.eulerAngles.x -= (transform.eulerAngles.x - (initRotation.x + currZoom * zoomRotation)) * 0.1;
	}

}
