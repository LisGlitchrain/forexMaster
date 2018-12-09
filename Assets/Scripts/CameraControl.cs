using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private float speed;
	private float zoom;
	private float orthoSize;
	Camera cam;
	// Component cam = Camera.main.GetComponent<Camera>;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//speed = GameManager.instance.gameSpeed;
		//if (speed > 14.0f && speed < 19.5f){ //Зачем это?

		//	zoom = speed/4.6f;
		//	orthoSize = Camera.main.orthographicSize;
		//	Camera.main.orthographicSize =  Mathf.Lerp(orthoSize, zoom, 3.0f);

		//	// Debug.Log(speed);
		//	// Debug.Log(zoom);
		//}
	}
}
