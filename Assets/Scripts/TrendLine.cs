using UnityEngine;
using System.Collections;

public class TrendLine : MonoBehaviour {

	public GameObject coin;
	private float height;
	private float currentHeight;
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

		particle = GetComponent<ParticleSystem>() as ParticleSystem;

		particle.startSpeed = GameManager.instance.gameSpeed/1.8f; 


	}
}
