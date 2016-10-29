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

		GameManager.instance.coinPosY = posY;
		GameManager.instance.influence = coinInfluence;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "LowerCol")
			{
				GameManager.instance.OutOfBounds (true, true);
			} 


		if (coll.gameObject.tag == "UpperCol")
			{
				GameManager.instance.OutOfBounds (false, true);
			} 
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "LowerCol")
			{
				GameManager.instance.OutOfBounds (true, false);
			} 


		if (coll.gameObject.tag == "UpperCol")
			{
				GameManager.instance.OutOfBounds (false, false);
			} 
	}


	}
