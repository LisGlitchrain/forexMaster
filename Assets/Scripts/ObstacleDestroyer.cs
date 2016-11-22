using UnityEngine;
using System.Collections;

public class ObstacleDestroyer : MonoBehaviour {

	public GameObject obstacleEater;
	private float candleX = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.x < obstacleEater.transform.position.x)
		{
			Destroy (gameObject);
		}

		else
		{
			candleX = transform.position.x;
			candleX -= GameManager.instance.gameSpeed/112.5f;
			// Debug.Log (candleX);
			transform.position = new Vector3 (candleX, transform.position.y, transform.position.z);
		}
	
	}
}
