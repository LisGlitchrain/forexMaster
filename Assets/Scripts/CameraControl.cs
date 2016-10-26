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
		speed = GameManager.instance.gameSpeed;
		if (speed > 12.0f && speed < 16.0f){

			zoom = speed/1.66666666667f;
			orthoSize = Camera.main.orthographicSize;
			Camera.main.orthographicSize =  Mathf.Lerp(orthoSize, zoom, 5.0f*speed);

			// Debug.Log(speed);
			// Debug.Log(zoom);
		}
	}
}
