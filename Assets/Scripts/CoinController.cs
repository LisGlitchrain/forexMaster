using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinController : MonoBehaviour {
	float oldRotZ;
	float newRotZ;
	float rotZ;
	public float posY;
	float coinInfluence;

	// Use this for initialization
	void Start () {
		coinInfluence = GameManager.instance.influenceMax;
	}
	
	// Update is called once per frame
	void Update () {
		// oldRotZ = newRotZ
		// newRotZ = -50 * Background.current.speed;
		// rotZ = Mathf.Lerp(oldRotZ, newRotZ, 10.0f*Time.deltaTime);
		// gameSpeed = GameManager.instance.gameSpeed;
		GameManager.instance.influenceBar.size = coinInfluence/GameManager.instance.influenceMax;

		if (coinInfluence < GameManager.instance.influenceMax) coinInfluence += GameManager.instance.influenceRiseSpeed/100;
		rotZ = -1.0f * GameManager.instance.gameSpeed;
		transform.Rotate(0, 0, rotZ);

		if(GameManager.instance.gameSpeed > 6.0f) GameManager.instance.gameSpeed -= 0.005f;

		// fallY = fallY-=;
		transform.position = new Vector3(1, posY-=GameManager.instance.coinFallSpeed/100, 0);
		
		if (Input.GetMouseButton(0) || Input.touchCount > 0){
			if (coinInfluence > 0.0f){ 
				posY+=GameManager.instance.coinRiseSpeed/100;
				coinInfluence -= GameManager.instance.influenceFallSpeed/100;
				if(GameManager.instance.gameSpeed < 18.0f) GameManager.instance.gameSpeed += 0.01f;
			}

			if (coinInfluence < 0.01f) Input.ResetInputAxes();

		}
            
			// Background.current.speed = gameSpeed;
			// Debug.Log(influence*100);
			// Debug.Log("GAMESPEED: " + GameManager.instance.gameSpeed);
		GameManager.instance.coinPosY = posY;
		GameManager.instance.influence = coinInfluence;
	}

	void OnCollisionEnter2d(Collision2D collider)
	{
		Debug.Log(collider.gameObject.tag);

		// if (collision.gameObject.tag == "UpperCol") Debug.Log("HIT");
	}

	}
