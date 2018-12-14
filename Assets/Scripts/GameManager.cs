using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;	

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public bool firstRun;
	public float gameSpeed;             //Общая скорость игры //public
    Timer timer;
    float gameSpeedPause;
    GS gameState;
    public bool gameOver; //public
    public int cycleCounter;            //Глобальный счётчик длительности игровой сессии //public

    [SerializeField] CoinController coin;
    [SerializeField] Economics economics;

    //Economics
    //public float influenceMax;                  //Максимальное количество очков влияния //public
    //public float influenceRiseSpeed;            //Скорость восстановления очков влияния //public
    //public float influenceFallSpeed;            //Скорость растраты очков влияния       //public
    //[SerializeField] float deposit;             //Объём депозита (денег для торговли)
    //[SerializeField] float initialPrice;        //Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
    //[SerializeField] float currentPrice;        //Текущая цена инструмента
    //[SerializeField] float supportPrice;        //Значение цены уровня сопротивления (верхняя красная граница)
    //[SerializeField] float resistancePrice;     //Значение цены уровня поддержки (нижняя зелёная граница)
    //[SerializeField] float openPrice;           //Цена открытия позиции (начала сделки)
    //[SerializeField] float closePrice;          //Цена закрытия позиции (конца сделки)
    //[SerializeField] float profit;              //Доход от сделки
    //[SerializeField] int quantity;				//Количество товара торгуемого инстумента
    //[SerializeField] float spread;              //Коэффициент разницы цен между ценой сопротивления и поддержки
    //[SerializeField] float comission;           //Размер комиссии, отчисляемой брокеру при открытии позиции
    //public float influence;                     //Текущее количество очков влияния //public
    //[SerializeField] float Devaluation;         //Коэффициент обесценивания инструмента при падении ниже линии поддержки
    //float stock;
    //float newSpread;
    //int newQuantity;

    [SerializeField] float topPositionProfit;       //Самый большой доход по сделке
    [SerializeField] float topSessionProfit;        //Самый большой доход за сессию 
    [SerializeField] float overallCycles;           //Длительность игровой сессии
    [SerializeField] float experience;          //Накопленный опыт
    [SerializeField] float finalScore;			//Количество очков опыта


    //??
    bool positionOpen;
	bool buying;
	bool underSupport;
	bool aboveResistance;

    //Ui кажись
    //Audio
    [SerializeField] AudioClip[] audioClip;
	AudioSource audioPlayer;

    //UI
    Vector3 panelY;
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

    public enum GS
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
            PauseGame(gameSpeed);
        }
	}

	public void StartGame()
	{	
		gameOver = false;
        economics.StartEconomics();
        SetPrices();
        positionOpen = false;
		gameOverPanel.gameObject.SetActive(false);
		gamePausedPanel.gameObject.SetActive(false);
        timer.StartTimer();
	}

	void Update() 
	{
        coin.CoinUpdate(economics.influenceMax,economics.influenceRiseSpeed, economics.influenceFallSpeed, Time.deltaTime, gameSpeed);
        economics.influence = coin.coinInfluence;
        economics.UpdateCurrentPrice(coin);
        economics.ProfitDepositMath(gameState, positionOpen, buying);
        if (underSupport == true)
		{
            economics.Devaluation(Time.deltaTime);
		}
		if (aboveResistance == true)
		{
            economics.MaxInfluenceDevaluation(Time.deltaTime);
		}
        //stats
		if (cycleCounter == 60)
		{
			ProgressStorage (1, 0);
			cycleCounter = 0;
		}

        //myUI
        influenceBar.size = coin.coinInfluence / economics.influenceMax; //ui
        //ui
        panelY = Camera.main.WorldToScreenPoint(new Vector3(-7.81f, coin.posY - 3.05f, 0.0f)); //нужна ли здесь panelY? НУЖНА
        currentPricePanel.localPosition = panelY;
        economics.PanelY = panelY;
        depositBar.size = economics.Deposit / 1000 * 5;
        //ui
        currentPriceText.text = economics.CurrentPrice.ToString();
        depositText.text = economics.RndDeposit.ToString(); //depositRnd.ToString();
        profitText.text = economics.profit.ToString();
        //ui
        if (economics.profit > 0) profitText.color = new Color(0, 182, 0);
        else if (economics.profit < 0) profitText.color = new Color(182, 0, 0);
        else profitText.color = new Color(240, 240, 240);
        //ui
        influenceText.text = economics.RoundInfluence().ToString();
        //ui and sound
        if (economics.Deposit < 20) { depositText.color = Color.red; SoundManager(4); }
		else depositText.color = new Color (0.49f, 0.49f, 0.49f);
        if (economics.Deposit <= 0 && gameOver == false) GameOver();
    }

    //mostlyUI
    public void OutOfBounds (bool support, bool stay)
	{
		if (gameState == GS.Play)
		{
			if (support == true && stay == true)
			{	
				underSupport = true; //notUI
				depositBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
				SoundManager(4);
			} 

			if (support == false && stay == true) 
			{
				aboveResistance = true;//notUI
                influenceBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
				SoundManager(4);
			}			

			if (support == true && stay == false) 
			{
				underSupport = false;//notUI
                depositBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
			}

			if (support == false && stay == false)
			{
                aboveResistance = false;//notUI
                influenceBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
			}
		}

		// else Debug.Log("RanAway");
	}

	public void SetQuantity (int setQuantity)
	{
			if (positionOpen == false)
			{
                economics.SetQuantity(setQuantity);
			}
		quantityText.text = economics.Quantity.ToString();
		SoundManager(3);
	}

	public void OpenBuyPosition ()
	{
		if (positionOpen == false) 
		{
			PositionManager(true, true, economics.CurrentPrice, economics.Quantity); //ui?
			openPositionPricePanel.localPosition = panelY;
		}
		else 
		{
			PositionManager(false, true, economics.CurrentPrice, 0); //ui?
            openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0); //ui
			if (economics.profit > 0) SoundManager(1);
			else  SoundManager(2);  
		}
	}

    //ui?
	public void OpenSellPosition ()
	{
		if (economics.PositionOpen == false) 
		{
			PositionManager(true, false, economics.CurrentPrice, economics.Quantity);
			openPositionPricePanel.localPosition = panelY;
			Debug.Log(panelY);
			SoundManager(0);
		}
		else 
		{
			PositionManager(false, false, economics.CurrentPrice, 0);
			openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0);
			if (economics.profit > 0) SoundManager(1);
			else  SoundManager(2); 
		}
	}
    // Здесь что-то страшное происходит Надо пересмотреть метод
	public void PositionManager (bool open, bool buy, float price, int quantity)
	{
		if ((price*quantity) <= economics.Deposit)
		{
			if (open == true)
			{
				if (buy == true) buying = true; else buying = false;
                economics.OpenPrice = price;
				economics.stock = economics.OpenPrice*quantity;
				//float oldDeposit = economics.Deposit;
                economics.Deposit = economics.Deposit - (economics.OpenPrice*quantity); //oldDeposit - ...
				positionOpen = true; 
				openPositionNumText.text = economics.OpenPrice.ToString(); //ui
				SoundManager(0);
				SoundManager(0);
			}
			if (open == false)
			{
                economics.OpenPrice = price;
                economics.Deposit += (economics.profit+economics.stock);
				positionOpen = false;
                openPositionNumText.text = economics.OpenPrice.ToString(); //ui
                ProgressStorage(0, economics.profit);
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

    //ui
    public void SetPrices ()
    {   
        resistancePriceText.text = economics.ResistancePrice.ToString();
        supportPriceText.text = economics.SupportPrice.ToString();
        currentPriceText.text = economics.CurrentPrice.ToString();
    }

	public void PauseButton()
	{
		if (gameState == GS.Play)
		{
			PauseGame(0);
			gameState = GS.Pause;
			gamePausedPanel.gameObject.SetActive(true);
		}
		else if (gameState == GS.Pause)
		{
			ResumeGame();
			gamePausedPanel.gameObject.SetActive(false);
		}
	}

	public void PauseGame (float gameSpd) 
    {
        gameSpeedPause = gameSpeed;
        gameSpeed = gameSpd;
        gameState = GS.Pause; //не могу проверить паузу, но кажись эта строка должна быть тут. По логике метода PauseButton
        timer.PauseTimer();
        coin.Pause();
        economics.Pause();
	}

	public void ResumeGame()
	{
		gameSpeed = gameSpeedPause;
		gameState = GS.Play;
        timer.ResumeTimer();
        coin.Resume();
        economics.Resume();
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
		PauseGame(0);
		economics.Comission = 0;
		if (positionOpen == true) economics.Deposit = 0;
		economics.profit = 0;
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
