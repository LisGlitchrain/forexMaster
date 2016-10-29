using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;	

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public bool firstRun;

	public float gameSpeed;
	public float influenceMax;
	public float influenceRiseSpeed;
	public float influenceFallSpeed;
	public float coinFallSpeed;
	public float coinRiseSpeed;
	public float deposit;
	public float initialPrice;
	public float currentPrice;
	public float supportPrice;
	public float resistancePrice;
	public float openPrice;
	public float closePrice;
	public float profit;
	public int quantity;
	public int cycleCounter;
	public float coinPosY;
	public float spread;
	public float comission;
	public float influence;
	public float Devaluation;
	float coinFallSpeedStorage;
	float coinRiseSpeedStorage;

	public float topPositionProfit;
	public float topSessionProfit;
	public float overallCycles;
	public float experience;
	public float finalScore;

	float stock;
	float newSpread;
	int newQuantity;
	bool gameOver;
	bool positionOpen;
	bool buying;
	bool underSupport;
	bool aboveResistance;
	Vector3 panelY;

	public AudioClip[] audioClip;
	AudioSource audioPlayer;
	
	public Scrollbar influenceBar;
	public Scrollbar depositBar;
	public Text depositText;
	public Text influenceText;
	public Text profitText;
	public Text openPositionNumText;
	public Text quantityText;
	public Text resistancePriceText;
	public Text supportPriceText;
	public Text currentPriceText;
	public RectTransform currentPricePanel;
	public RectTransform openPositionPricePanel;

	public Scrollbar expBar;
	public RectTransform gameOverPanel;
	public RectTransform dialogPanel;
	public Text tPPText;
	public Text tSPText;
	public Text sLenghtText;
	public Text expText;


	void Awake()
	{	
		// if (instance = null) instance = this;
		// else if (instance != this) Destroy(gameObject);
		// DontDestroyOnLoad(gameObject);
		instance = this;
		StartGame();
		audioPlayer = GetComponent<AudioSource>();
		gameOver = false;

	}

	void PreRun()
	{

	}

	public void StartGame()
	{	
		gameOver = false;
		currentPrice = initialPrice;
		SetPrices(initialPrice);
		positionOpen = false;
		quantity = 1;
		gameOverPanel.gameObject.SetActive(false);
	}

	void Update() 
	{
		currentPrice = Mathf.Round((initialPrice+coinPosY*(1/newSpread))*100.0f)/100.0f;
		currentPriceText.text=currentPrice.ToString();

		float depositRnd = Mathf.Round((deposit*100.0f)/100.0f); 
		depositText.text = depositRnd.ToString();

		profitText.text = profit.ToString();
		if (profit > 0) profitText.color = new Color(0, 182, 0);
		else if (profit < 0) profitText.color = new Color(182, 0, 0);
		else profitText.color = new Color(240, 240, 240);

		if (positionOpen == true && buying == true)
		{
			profit = Mathf.Round(((currentPrice-openPrice)*quantity)*100.0f)/100.0f;
			deposit-=comission;
			Debug.Log("OP: " + openPrice + " | Current Price: " + currentPrice + " | Profit: " + profit);
		}

		else if (positionOpen == true && buying == false) 
		{
			profit = Mathf.Round(((openPrice-currentPrice)*quantity)*100.0f)/100.0f;
			deposit-=comission;
			Debug.Log("OP: " + openPrice + " | Current Price: " + currentPrice + " | Profit: " + profit);
		}

		else profit = 0;
		
		float oldDeposit = deposit;
		deposit = Mathf.Round(oldDeposit*100)/100;
		panelY = Camera.main.WorldToScreenPoint(new Vector3 (-7.81f, coinPosY-3.05f, 0.0f));
		currentPricePanel.localPosition = panelY;

		// if (deposit < 20) SoundManager(4); 
		if (deposit <= 0 && gameOver == false) GameOver();
		depositBar.size = deposit/1000*5;

		float influenceRnd = Mathf.Round((influence*100.0f)/100.0f);
		influenceText.text = influenceRnd.ToString();

		if (underSupport == true)
		{
			deposit -= Mathf.Lerp(0, Devaluation, Time.deltaTime * 5.0f);
		}

		if (aboveResistance == true)
		{
			influenceMax -= Mathf.Lerp(0, Devaluation, Time.deltaTime * 5.0f);
		}

		if (cycleCounter == 60)
		{
			ProgressStorage (1, 0);
			cycleCounter = 0;
		}
	}
	// public void initParameters (float gameSpeed, float influenceMax, float influenceRiseSpeed, float influenceFallSpeed, float coinFallSpeed, float coinRiseSpeed) 
	// {

	// }

	public void OutOfBounds (bool support, bool stay)
	{

		if (support == true && stay == true)
		{	
			coinFallSpeedStorage = coinFallSpeed;
			coinFallSpeed = 0;
			underSupport = true;
		} 

		if (support == false && stay == true) 
		{
			coinRiseSpeedStorage = coinRiseSpeed;
			coinRiseSpeed = 0;
			aboveResistance = true;
		}
		

		if (support == true && stay == false) 
		{
			coinFallSpeed = coinFallSpeedStorage;
			underSupport = false;
		}

		if (support == false && stay == false)
		{
			coinRiseSpeed = coinRiseSpeedStorage;
			aboveResistance = false;
		}

		// else Debug.Log("RanAway");
	}

	public void SetQuantity (int setQuantity)
	{
			if (positionOpen == false)
			{
				newQuantity = quantity += setQuantity;
				if (newQuantity > 1) {
					quantity = newQuantity;
				}

				else quantity = 1;
			}
		quantityText.text = quantity.ToString();
		SoundManager(3);
	}

	public void OpenBuyPosition ()
	{
		if (positionOpen == false) 
		{
			PositionManager(true, true, currentPrice, quantity);
			openPositionPricePanel.localPosition = panelY;
		}

		else 
		{
			PositionManager(false, true, currentPrice, 0);
			openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0);
			if (profit > 0) SoundManager(1);
			else  SoundManager(2);  
		}
	}


	public void OpenSellPosition ()
	{
		if (positionOpen == false) 
		{
			PositionManager(true, false, currentPrice, quantity);
			openPositionPricePanel.localPosition = panelY;
			Debug.Log(panelY);
			SoundManager(0);
		}

		else 
		{
			PositionManager(false, false, currentPrice, 0);
			openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0);
			if (profit > 0) SoundManager(1);
			else  SoundManager(2); 
		}
	}

	public void PositionManager (bool open, bool buy, float price, int quantity)
	{
		if ((price*quantity) <= deposit)
		{
			if (open == true)
			{
				if (buy == true) buying = true; else buying = false;
				openPrice = price;
				stock = openPrice*quantity;
				{
					float oldDeposit = deposit;
					deposit = oldDeposit - (openPrice*quantity);
					openPositionNumText.text = openPrice.ToString();
					positionOpen = true;
					SoundManager(0);
				}
				SoundManager(0);
			}

			if (open == false)
			{
				openPrice = price;
				{
					deposit+=(profit+stock);
					openPositionNumText.text = openPrice.ToString();
					positionOpen = false;
					ProgressStorage(0, profit);
				}
			}
		}

		else 
		{
			Debug.Log("No money, no honey!");
			openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0);
		}
	}

	public void SoundManager(int clip)
	{
		audioPlayer.clip = audioClip[clip];
		audioPlayer.Play();
	}

	public void SetPrices (float price){

		initialPrice = Mathf.Round(price * 100f) / 100f;
		resistancePrice = Mathf.Round((price*spread) * 100f) / 100f;
		supportPrice = Mathf.Round((price/spread) * 100f) / 100f;
		resistancePriceText.text = resistancePrice.ToString();
		supportPriceText.text = supportPrice.ToString();
		currentPriceText.text = currentPrice.ToString();
		newSpread = resistancePrice-supportPrice;

	}

	public void PauseGame (float coinFallSpd, float coinRsSpd, float infFallSpd, float gameSpd)
	{
		coinFallSpeed = coinFallSpd;
		coinRiseSpeed = coinRsSpd;
		influenceFallSpeed = infFallSpd;
		gameSpeed = gameSpd;	
	}

	void ProgressStorage (float cycles, float tPP)
	{
		overallCycles+=cycles;
		if (tPP > topPositionProfit) topPositionProfit = tPP;
		topSessionProfit+=tPP;
	}

	void ScoreCounter (float cycles, float tPP, float tSP)
	{
		finalScore = cycles+tPP+tSP;
	}

	void GameOver ()
	{	
		gameOver = true;
		gameOverPanel.gameObject.SetActive(true);
		SoundManager(5);
		PauseGame(4, 0, 0, 0);
		comission = 0;
		deposit = 0;
		profit = 0;
		ScoreCounter (overallCycles, topPositionProfit, topSessionProfit);
		positionOpen = false;
		// experience += finalScore;

		tPPText.text = topPositionProfit.ToString();
		tSPText.text = topSessionProfit.ToString();
		sLenghtText.text = overallCycles.ToString();
		expText.text = finalScore.ToString();
		expBar.size = finalScore/100;
		Debug.Log("OVERALL SCORE:  " + finalScore + "  || Cycles: " + overallCycles + " || TPP: " + topPositionProfit + " || TSP: " + topSessionProfit + "\n EXPERIENCE: " + experience);
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene (0);
		// gameOverPanel.gameObject.SetActive(false);
	}
}
