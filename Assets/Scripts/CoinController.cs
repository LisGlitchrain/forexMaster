using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinController : MonoBehaviour, IPauseble
{
	float oldRotZ;
	float newRotZ;
	float rotZ;
	public float posY;
	public float coinInfluence;

    float coinFallSpeed;         //Скорость падения монеты //public 
    float coinRiseSpeed;         //Скорость подъёма монеты //public
    float pausedCoinFallSpeed;
    float pausedCoinRiseSpeed;

    [SerializeField] float maxCoinFallSpeed;
    [SerializeField] float maxCoinRiseSpeed;

    // Use this for initialization
    void Start ()
    {

	}

    public void StartCoin(float influenceMax, float maxCoinFallSpeed, float maxCoinRiseSpeed)
    {
        coinInfluence = influenceMax;
        coinFallSpeed = maxCoinFallSpeed;
        coinRiseSpeed = maxCoinRiseSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {

		//GameManager.instance.influenceBar.size = coinInfluence/GameManager.instance.influenceMax;

		//if (coinInfluence < GameManager.instance.influenceMax) coinInfluence += GameManager.instance.influenceRiseSpeed * Time.deltaTime; // / 100
		//rotZ = -100.0f * GameManager.instance.gameSpeed * Time.deltaTime;
		//transform.Rotate(0, 0, rotZ);

  //      if (Input.GetMouseButton(0) || Input.touchCount > 0){
		//	if (coinInfluence > 0.0f){ 
		//		posY+=coinRiseSpeed * Time.deltaTime;// / 100
  //              coinInfluence -= GameManager.instance.influenceFallSpeed* Time.deltaTime; // / 100 //economics
  //          }

		//	if (coinInfluence < 0.01f) Input.ResetInputAxes(); //Зачем?

		//}
  //      else
  //      {
  //          posY -= coinFallSpeed * Time.deltaTime;
  //      }
  //      transform.position = new Vector3(1, posY, 0); // / 100

		//GameManager.instance.influence = coinInfluence;
	}

    public void CoinUpdate(float influenceMax, float influenceRiseSpeed, float influenceFallSpeed, float deltaTime, float gameSpeed)
    {


        if (coinInfluence < influenceMax) coinInfluence += influenceRiseSpeed * deltaTime; // / 100
        rotZ = -100.0f * gameSpeed * deltaTime;
        transform.Rotate(0, 0, rotZ);

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            if (coinInfluence > 0.0f)
            {
                posY += coinRiseSpeed * deltaTime;// / 100
                coinInfluence -= influenceFallSpeed * deltaTime; // / 100 //economics
            }

            if (coinInfluence < 0.01f) Input.ResetInputAxes(); //Зачем?

        }
        else
        {
            posY -= coinFallSpeed * deltaTime;
        }
        transform.position = new Vector3(1, posY, 0); // / 100


    }

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "LowerCol")
			{
                coinFallSpeed = 0;
                GameManager.instance.OutOfBounds (true, true);
			} 


		if (coll.gameObject.tag == "UpperCol")
			{
                coinRiseSpeed = 0;
                GameManager.instance.OutOfBounds (false, true);
				Input.ResetInputAxes();
			}

		if (coll.gameObject.tag == "ObstacleCol")
			{
				GameManager.instance.GameOver ();
			} 
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "LowerCol")
			{
                coinFallSpeed = maxCoinFallSpeed;

                GameManager.instance.OutOfBounds (true, false);
			} 


		if (coll.gameObject.tag == "UpperCol")
			{
                coinRiseSpeed = maxCoinRiseSpeed;
                GameManager.instance.OutOfBounds (false, false);
			} 
	}

    public void Pause()
    {
        pausedCoinRiseSpeed = coinRiseSpeed;
        pausedCoinFallSpeed = coinFallSpeed;
        coinRiseSpeed = 0f;
        coinFallSpeed = 0f;
    }

    public void Resume()
    {
        coinRiseSpeed = pausedCoinRiseSpeed;
        coinFallSpeed = pausedCoinFallSpeed;
    }
}
