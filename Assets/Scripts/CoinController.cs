using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float posYzero;
    public float posY;
	float rotZ;

    // Use this for initialization
    void Start ()
    {

	}

    /// <summary>
    /// Initialises coin.
    /// </summary>
    /// <param name="influenceMax"></param>
    public void StartCoin(float influenceMax)
    {
        posYzero = transform.position.y;
    }
    /// <summary>
    /// Update coin position according economics(current price.)
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="gameSpeed"></param>
    /// <param name="currentPrice">Have to be received drom econommics.</param>
    public void CoinUpdate(float deltaTime, float gameSpeed, float currentPrice)
    {
        rotZ = -100.0f * gameSpeed * deltaTime;
        transform.Rotate(0, 0, rotZ);
        posY = posYzero + currentPrice;
        transform.position = new Vector3(1, posY, 0); 
    }

	void OnTriggerEnter2D(Collider2D coll)
	{

		if (coll.gameObject.tag == "ObstacleCol")
		{
			GameManager.instance.GameOver ();
		} 
	}
}
