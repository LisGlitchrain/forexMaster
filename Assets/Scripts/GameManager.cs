using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;	

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public bool firstRun;

	public float gameSpeed;             //Общая скорость игры //public
    public float influenceMax;			//Максимальное количество очков влияния //public
	public float influenceRiseSpeed;    //Скорость восстановления очков влияния //public
    public float influenceFallSpeed;    //Скорость растраты очков влияния       //public
    public float coinFallSpeed;         //Скорость падения монеты //public
    public float coinRiseSpeed;         //Скорость подъёма монеты //public
    [SerializeField] float deposit;             //Объём депозита (денег для торговли)
    [SerializeField] float initialPrice;            //Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
    [SerializeField] float currentPrice;            //Текущая цена инструмента
    [SerializeField] float supportPrice;            //Значение цены уровня сопротивления (верхняя красная граница)
    [SerializeField] float resistancePrice;     //Значение цены уровня поддержки (нижняя зелёная граница)
    [SerializeField] float openPrice;               //Цена открытия позиции (начала сделки)
    [SerializeField] float closePrice;          //Цена закрытия позиции (конца сделки)
    [SerializeField] float profit;              //Доход от сделки
    [SerializeField] int quantity;				//Количество товара торгуемого инстумента
	public int cycleCounter;            //Глобальный счётчик длительности игровой сессии //public
    public float coinPosY;              //Значение позиции монеты по Y (техническая переменная) //public
    [SerializeField] float spread;              //Коэффициент разницы цен между ценой сопротивления и поддержки
    [SerializeField] float comission;				//Размер комиссии, отчисляемой брокеру при открытии позиции
	public float influence;             //Текущее количество очков влияния //public
    [SerializeField] float Devaluation;			//Коэффициент обесценивания инструмента при падении ниже линии поддержки
	float coinFallSpeedStorage;
    float coinRiseSpeedStorage;

    [SerializeField] float topPositionProfit;       //Самый большой доход по сделке
    [SerializeField] float topSessionProfit;        //Самый большой доход за сессию 
    [SerializeField] float overallCycles;           //Длительность игровой сессии
    [SerializeField] float experience;          //Накопленный опыт
    [SerializeField] float finalScore;			//Количество очков опыта

	float stock;						
	float newSpread;
	int newQuantity;
	GS gameState;
	public bool gameOver; //public
    bool positionOpen;
	bool buying;
	bool underSupport;
	bool aboveResistance;
	Vector3 panelY;

    [SerializeField] AudioClip[] audioClip;
	AudioSource audioPlayer;

    [SerializeField] List <float> pauseGameList = new List <float>();
	
	public Scrollbar influenceBar; //public
    [SerializeField] Scrollbar depositBar;
    [SerializeField] Text depositText;
    [SerializeField] Text influenceText;
    [SerializeField] Text profitText;
    [SerializeField] Text openPositionNumText;
    [SerializeField] Text quantityText;
    [SerializeField] Text resistancePriceText;
    [SerializeField] Text supportPriceText;
    [SerializeField] Text currentPriceText;
    [SerializeField] RectTransform currentPricePanel;
    [SerializeField] RectTransform openPositionPricePanel;

    [SerializeField] Scrollbar expBar;
    [SerializeField] RectTransform gameOverPanel;
    [SerializeField] RectTransform gamePausedPanel;
    [SerializeField] RectTransform mainTutorialPanel;
    [SerializeField] Text tPPText;
    [SerializeField] Text tSPText;
    [SerializeField] Text sLengthText;
    [SerializeField] Text expText;

    Timer timer;

    enum GS
    {
        Play,
        Pause,
        Over
    }


	void Awake()
	{	

		instance = this;
        timer = GetComponent<Timer>();
        StartGame();
		audioPlayer = GetComponent<AudioSource>();
		gameOver = false;
		PreRun ();
        
	}

	void PreRun()
	{
		if (firstRun == true)
        {
            mainTutorialPanel.gameObject.SetActive(true);
            mainTutorialPanel.GetComponent<Tutorial>().RunTutorial(0);
            PauseGame(0, 0, influenceRiseSpeed, influenceFallSpeed, gameSpeed);
        }
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
        timer.StartTimer();
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
			if (gameState == GS.Play) deposit-=comission;
			Debug.Log("OP: " + openPrice + " | Current Price: " + currentPrice + " | Profit: " + profit);
		}

		else if (positionOpen == true && buying == false) 
		{
			profit = Mathf.Round(((openPrice-currentPrice)*quantity)*100.0f)/100.0f;
			if (gameState == GS.Play) deposit-=comission;
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

	public void OutOfBounds (bool support, bool stay)
	{
		if (gameState == GS.Play)
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
		audioPlayer.clip = audioClip[clip]; //тут что-то сломалось
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

	public void PauseButton()
	{
		if (gameState == GS.Play)
		{
			PauseGame(0, 0, 0, 0, 0);
			gameState = GS.Pause;
			gamePausedPanel.gameObject.SetActive(true);

		}

		else if (gameState == GS.Pause)
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
        timer.PauseTimer();
	}

	public void ResumeGame()
	{
		influenceRiseSpeed = pauseGameList[0];
		coinFallSpeed = pauseGameList[1];
		coinRiseSpeed = pauseGameList[2];
		influenceFallSpeed = pauseGameList[3];
		gameSpeed = pauseGameList[4];
		gameState = GS.Play;
		pauseGameList.Clear();
        timer.ResumeTimer();
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
		if (positionOpen == true) deposit = 0;
		profit = 0;
		ScoreCounter (overallCycles, topPositionProfit, topSessionProfit);
		positionOpen = false;
		// experience += finalScore;

		tPPText.text = topPositionProfit.ToString();
		tSPText.text = topSessionProfit.ToString();
		sLengthText.text = timer.RoundedTimeSecs().ToString(); 
		expText.text = finalScore.ToString();
		expBar.size = finalScore/100;

        timer.PauseTimer();
        Debug.Log("OVERALL SCORE:  " + finalScore + "  || Cycles: " + overallCycles + "  || Timer: " + timer.RoundedTimeSecs()
            + " || TPP: " + topPositionProfit + " || TSP: " + topSessionProfit + "\n EXPERIENCE: " + experience);
		gameState = GS.Over;
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene (0);
	}
}
