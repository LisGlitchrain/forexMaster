using UnityEngine;
using System.Collections;

public class TrendLine : MonoBehaviour {

	public GameObject coin;
	private float height;
	private float currentHeight;
	float gameSpeed;
	ParticleSystem particle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		height = coin.transform.position.y;
		currentHeight = transform.position.y;
		currentHeight = height;

		transform.position = new Vector2(coin.transform.position.x,currentHeight);

		gameSpeed = GameManager.instance.gameSpeed;
		// gameObject.ParticleSystem.startSpeed = gameSpeed;
		particle = GetComponent<ParticleSystem>() as ParticleSystem;

		// gameSpeed = Background.current.speed;
		particle.startSpeed = gameSpeed/1.8f; 


	}
}
