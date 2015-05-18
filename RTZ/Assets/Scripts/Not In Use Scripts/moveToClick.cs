using UnityEngine;
using System.Collections;

public class moveToClick : MonoBehaviour 
{
//	public float duration = 50.0f;
//
//	bool flag = false;
//	Vector3 newPos;
//	float yAxis;
//
//	void Start()
//	{
//		yAxis = gameObject.transform.position.y;
//	}
//
//	void Update()
//	{
//		if (Input.GetMouseButtonDown (1)) {
//			RaycastHit hit;
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//
//			if (Physics.Raycast(ray, out hit)) {
//				flag = true;
//				newPos = hit.point;
//				newPos.y = yAxis;
//			}
//		}
//
//		if (flag) {
//			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newPos, 1/(duration * (Vector3.Distance(gameObject.transform.position, newPos))));
//		}
//	}
	

	public float speed;

	Vector3 newPos;
	float yAxis;
	bool move = false;

	void Start()
	{
		yAxis = transform.position.y;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				newPos = hit.point;
				newPos.y = yAxis;
				move = true;
			}
		}

		if (move) {
			transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * speed);
		} else if (newPos == transform.position){
			move = false;
		}
	}
}
