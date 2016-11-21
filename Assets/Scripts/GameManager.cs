using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;	

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public bool firstRun;

	public float gameSpeed;				//Общая скорость игры
	public float influenceMax;			//Максимальное количество очков влияния
	public float influenceRiseSpeed;	//Скорость восстановления очков влияния
	public float influenceFallSpeed;	//Скорость растраты очков влияния
	public float coinFallSpeed;			//Скорость падения монеты
	public float coinRiseSpeed;			//Скорость подъёма монеты
	public float deposit;				//Объём депозита (денег для торговли)
	public float initialPrice;			//Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
	public float currentPrice;			//Текущая цена инструмента
	public float supportPrice;			//Значение цены уровня сопротивления (верхняя красная граница)
	public float resistancePrice;		//Значение цены уровня поддержки (нижняя зелёная граница)
	public float openPrice;				//Цена открытия позиции (начала сделки)
	public float closePrice;			//Цена закрытия позиции (конца сделки)
	public float profit;				//Доход от сделки
	public int quantity;				//Количество товара торгуемого инстумента
	public int cycleCounter;			//Глобальный счётчик длительности игровой сессии
	public float coinPosY;				//Значение позиции монеты по Y (техническая переменная)
	public float spread;				//Коэффициент разницы цен между ценой сопротивления и поддержки
	public float comission;				//Размер комиссии, отчисляемой брокеру при открытии позиции
	public float influence;				//Текущее количество очков влияния
	public float Devaluation;			//Коэффициент обесценивания инструмента при падении ниже линии поддержки
	float coinFallSpeedStorage;			
	float coinRiseSpeedStorage;

	public float topPositionProfit;		//Самый большой доход по сделке
	public float topSessionProfit;		//Самый большой доход за сессию 
	public float overallCycles;			//Длительность игровой сессии
	public float experience;			//Накопленный опыт
	public float finalScore;			//Количество очков опыта

	float stock;						
	float newSpread;
	int newQuantity;
	int gameState;
	// int tutorStep;
	bool gameOver;
	bool positionOpen;
	bool buying;
	bool underSupport;
	bool aboveResistance;
	Vector3 panelY;

	public AudioClip[] audioClip;
	AudioSource audioPlayer;

	public List <float> pauseGameList = new List <float>(); 
	public RectTransform[] tutorialPanels;
	
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
	public RectTransform gamePausedPanel;
	public RectTransform mainTutorialPanel;
	public Text tPPText;
	public Text tSPText;
	public Text sLengthText;
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
		PreRun ();

	}

	void PreRun()
	{
		if (firstRun == true) { RunTutorial (0); PauseGame(0, 0, influenceRiseSpeed, influenceFallSpeed, gameSpeed);}
	}

	public void StartGame()
	{	
		gameOver = false;
		currentPrice = initialPrice;
		SetPrices(initialPrice);
		positionOpen = false;
		quantity = 1;
		gameOverPanel.gameObject.SetActive(false);
		gamePausedPanel.gameObject.SetActive(false);
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
			if (gameState == 0) deposit-=comission;
			Debug.Log("OP: " + openPrice + " | Current Price: " + currentPrice + " | Profit: " + profit);
		}

		else if (positionOpen == true && buying == false) 
		{
			profit = Mathf.Round(((openPrice-currentPrice)*quantity)*100.0f)/100.0f;
			if (gameState == 0) deposit-=comission;
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

		if (deposit < 20) { depositText.color = Color.red; SoundManager(4); }
		else depositText.color = new Color (0.49f, 0.49f, 0.49f);
		
	}
	// public void initParameters (float gameSpeed, float influenceMax, float influenceRiseSpeed, float influenceFallSpeed, float coinFallSpeed, float coinRiseSpeed) 
	// {

	// }

	public void OutOfBounds (bool support, bool stay)
	{
		if (gameState == 0)
		{
			if (support == true && stay == true)
			{	
				coinFallSpeedStorage = coinFallSpeed;
				coinFallSpeed = 0;
				underSupport = true;
				depositBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
				SoundManager(4);
			} 

			if (support == false && stay == true) 
			{
				coinRiseSpeedStorage = coinRiseSpeed;
				coinRiseSpeed = 0;
				aboveResistance = true;
				influenceBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
				SoundManager(4);

			}
			

			if (support == true && stay == false) 
			{
				coinFallSpeed = coinFallSpeedStorage;
				underSupport = false;
				depositBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
			}

			if (support == false && stay == false)
			{
				coinRiseSpeed = coinRiseSpeedStorage;
				aboveResistance = false;
				influenceBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
			}
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

	public void RunTutorial(int tutorStep)
	{
		if (tutorStep == 0)
		{
			mainTutorialPanel.gameObject.SetActive(true);
			tutorialPanels[0].gameObject.SetActive(true);
			tutorStep = 1;
		} 

		else if (tutorStep == 1)
		{
			tutorialPanels[0].gameObject.SetActive(false);
			tutorialPanels[1].gameObject.SetActive(true);
			tutorStep = 2;
		}

		else if (tutorStep == 2)
		{
			tutorialPanels[1].gameObject.SetActive(false);
			tutorialPanels[2].gameObject.SetActive(true);
			tutorStep = 3;
		}

		else if (tutorStep == 3)
		{
			tutorialPanels[2].gameObject.SetActive(false);
			tutorialPanels[3].gameObject.SetActive(true);
			tutorStep = 4;
		}		

		else if (tutorStep == 4)
		{
			tutorialPanels[3].gameObject.SetActive(false);
			tutorialPanels[4].gameObject.SetActive(true);
			tutorStep = 5;
		}

		else if (tutorStep == 5)
		{
			tutorialPanels[4].gameObject.SetActive(false);
			tutorialPanels[5].gameObject.SetActive(true);
			tutorStep = 6;
		}

		else if (tutorStep == 6)
		{
			tutorialPanels[5].gameObject.SetActive(false);
			tutorialPanels[6].gameObject.SetActive(true);
			tutorStep = 7;
		}

		else if (tutorStep == 7)
		{
			tutorialPanels[6].gameObject.SetActive(false);
			firstRun = false;
			mainTutorialPanel.gameObject.SetActive(false);
			ResumeGame();
		}

		// else if (tutorStep == 8)
		// {
		// 	tutorialPanels[8].gameObject.SetActive(false);
		// 	firstRun = false;
		// 	mainTutorialPanel.gameObject.SetActive(false);
		// 	ResumeGame();
		// }

	}

	public void PauseButton()
	{
		if (gameState == 0)
		{
			PauseGame(0, 0, 0, 0, 0);
			gameState = 1;
			gamePausedPanel.gameObject.SetActive(true);

		}

		else if (gameState == 1)
		{
			ResumeGame();
			gamePausedPanel.gameObject.SetActive(false);
		}
	}

	public void PauseGame (float coinFallSpd, float coinRsSpd, float infFallSpd, float infRsSpd, float gameSpd)
	{	
		pauseGameList.Add (influenceRiseSpeed);
		pauseGameList.Add (coinFallSpeed);
		pauseGameList.Add (coinRiseSpeed);
		pauseGameList.Add (influenceFallSpeed);
		pauseGameList.Add (gameSpeed);

		influenceRiseSpeed = infRsSpd;
		coinFallSpeed = coinFallSpd;
		coinRiseSpeed = coinRsSpd;
		influenceFallSpeed = infFallSpd;
		gameSpeed = gameSpd;	
	}

	public void ResumeGame()
	{
		influenceRiseSpeed = pauseGameList[0];
		coinFallSpeed = pauseGameList[1];
		coinRiseSpeed = pauseGameList[2];
		influenceFallSpeed = pauseGameList[3];
		gameSpeed = pauseGameList[4];
		gameState = 0;
		pauseGameList.Clear();
	}

	public void RestartGame()
	{
		SceneManager.LoadScene (1);
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

	public void GameOver ()
	{	
		gameOver = true;
		gameOverPanel.gameObject.SetActive(true);
		SoundManager(5);
		PauseGame(4, 0, 0, 0, 0);
		comission = 0;
		deposit = 0;
		profit = 0;
		ScoreCounter (overallCycles, topPositionProfit, topSessionProfit);
		positionOpen = false;
		// experience += finalScore;

		tPPText.text = topPositionProfit.ToString();
		tSPText.text = topSessionProfit.ToString();
		sLengthText.text = overallCycles.ToString();
		expText.text = finalScore.ToString();
		expBar.size = finalScore/100;
		Debug.Log("OVERALL SCORE:  " + finalScore + "  || Cycles: " + overallCycles + " || TPP: " + topPositionProfit + " || TSP: " + topSessionProfit + "\n EXPERIENCE: " + experience);
		gameState = 2;
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene (0);
		// gameOverPanel.gameObject.SetActive(false);
	}
}
