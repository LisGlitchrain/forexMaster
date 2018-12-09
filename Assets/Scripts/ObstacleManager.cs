using UnityEngine;
using System.Collections;

public class ObstacleManager : MonoBehaviour {

    [SerializeField] GameObject candleCollider;
	[SerializeField] int frequency;

    GameObject lastSpawnedCandle;
    float candleHeight;
	float candleWidth;

	// Use this for initialization
	void Start () {
		candleWidth = candleCollider.GetComponent<BoxCollider2D>().size.x;
	}
	
	// Update is called once per frame
	void Update () {
        if (lastSpawnedCandle == null)
        {
            lastSpawnedCandle = new GameObject();
            lastSpawnedCandle.transform.position = new Vector3(0, 0, 0);
        }
		if (lastSpawnedCandle.transform.position.x + candleWidth < this.transform.position.x)
		{
		    if (GameManager.instance.gameOver == false)
		    {
				    int probability = Random.Range(0, frequency);
				    if (probability == 1) Spawn ();
		    }
		}
	}

	void Spawn () 
	{
		Debug.Log("SPAWN!");
		transform.position = new Vector3 (transform.position.x, Random.Range(3.0f,1.0f), 0);
		lastSpawnedCandle = Instantiate (candleCollider, transform.position, transform.rotation);
    }
}
