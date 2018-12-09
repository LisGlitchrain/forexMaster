using UnityEngine;
using System.Collections;

public class ObstacleDestroyer : MonoBehaviour {

	[SerializeField] GameObject obstacleEater;
    [SerializeField] float speed;

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
			candleX -= GameManager.instance.gameSpeed* Time.deltaTime * speed; // / 112.5f 
            // Debug.Log (candleX);
            transform.position = new Vector3 (candleX, transform.position.y, transform.position.z);
		}
	
	}
}
