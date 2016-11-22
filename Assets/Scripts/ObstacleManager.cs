using UnityEngine;
using System.Collections;

public class ObstacleManager : MonoBehaviour {

	public GameObject candleCollider;
	public float distance;
	public float maxGenerationQuantity;

	private float candleHeight;
	private float candleWidth;

	private float posX;

	// Use this for initialization
	void Start () {
		candleWidth = candleCollider.GetComponent<BoxCollider2D>().size.x;
		posX = transform.position.x;
	
	}
	
	// Update is called once per frame
	void Update () {
		// if (transform.position.x < generatorPoint.transform.position.x)
		// {
		if (GameManager.instance.gameOver == false)
		{
				int probability = Random.Range(0,50);
				if (probability == 1) Spawn ();
		}
		// }
	}

	void Spawn () 
		{
			Debug.Log("SPAWN!");

			for (float i = 0; i < Random.Range(0, maxGenerationQuantity); i++)
				{	
					posX+=candleWidth+distance;
					transform.position = new Vector3 (posX, Random.Range(3.0f,1.0f), 0);
					Instantiate (candleCollider, transform.position, transform.rotation);
				}
		}
}
