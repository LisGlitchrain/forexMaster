using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;	

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public bool firstRun;
	public float gameSpeed;             //Общая скорость игры //public
    public bool gameOver;               //public
    public int cycleCounter;            //Глобальный счётчик длительности игровой сессии //public
    Timer timer;
    float gameSpeedPause;
    GS gameState;

    [SerializeField] CoinController coin;
    [SerializeField] Economics economics;
    [SerializeField] UIManager uiManager;
    [SerializeField] float topPositionProfit;       //Самый большой доход по сделке
    [SerializeField] float topSessionProfit;        //Самый большой доход за сессию 
    [SerializeField] float overallCycles;           //Длительность игровой сессии
    [SerializeField] float experience;          //Накопленный опыт
    [SerializeField] float finalScore;			//Количество очков опыта

    //logic
    bool positionOpen;
	bool buying;
	bool underSupport;
	bool aboveResistance;

    //Audio
    [SerializeField] AudioClip[] audioClip;
	AudioSource audioPlayer;

    //UI
    Vector3 panelY;
    public Scrollbar influenceBar; //public //size, fade
    [SerializeField] Scrollbar depositBar; //size, fade
    [SerializeField] Text depositText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text influenceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text profitText;   //text
    [SerializeField] Text openPositionNumText;  //text
    [SerializeField] Text quantityText; //text
    [SerializeField] Text resistancePriceText;  //text
    [SerializeField] Text supportPriceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] Text currentPriceText; //text //можно получать из чайлда, а не напрямую
    [SerializeField] RectTransform currentPricePanel;   //yposition, 
    [SerializeField] RectTransform openPositionPricePanel; //ypos
    [SerializeField] Scrollbar expBar; //size
    [SerializeField] RectTransform gameOverPanel; //active
    [SerializeField] RectTransform gamePausedPanel;  //active
    [SerializeField] RectTransform mainTutorialPanel; //???
    [SerializeField] Text tPPText; //text
    [SerializeField] Text tSPText;  //text
    [SerializeField] Text sLengthText;  //text
    [SerializeField] Text expText;  //text

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
        coin.StartCoin(economics.influenceMax);
        positionOpen = false;
		gameOverPanel.gameObject.SetActive(false);
		gamePausedPanel.gameObject.SetActive(false);
        timer.StartTimer();
	}

	void Update() 
	{
        if (gameState == GS.Play)
        {
            coin.CoinUpdate(Time.deltaTime, gameSpeed, economics.PriceToDeltaPos);
            economics.UpdateCurrentPrice(Time.deltaTime, Input.touchCount, Input.GetMouseButton(0));
            economics.ProfitDepositMath(gameState, positionOpen, buying);
            economics.Devaluation(Time.deltaTime);
            economics.InfluenceDevaluation(Time.deltaTime);
            if (economics.GetEconomicChanged())
            {
                OutOfBounds(economics.EcoStatus);
            }
        }

        //stats
		if (cycleCounter == 60)
		{
			ProgressStorage (1, 0);
			cycleCounter = 0;
		}

        //myUI
        influenceBar.size = economics.influence / economics.influenceMax; //ui
        //ui
        panelY = Camera.main.WorldToScreenPoint(new Vector3(-7.81f, coin.posY - 3.05f, 0.0f)); //нужна ли здесь panelY? НУЖНА
        currentPricePanel.localPosition = panelY;
        economics.PanelY = panelY;
        depositBar.size = economics.Deposit / 1000 * 5;
        //ui
        currentPriceText.text = economics.CurrentPrice.ToString();
        depositText.text = economics.RndDeposit.ToString(); //depositRnd.ToString();
        profitText.text = economics.Profit.ToString();
        //ui
        if (economics.Profit > 0) profitText.color = new Color(0, 182, 0);
        else if (economics.Profit < 0) profitText.color = new Color(182, 0, 0);
        else profitText.color = new Color(240, 240, 240);
        //ui
        influenceText.text = economics.RoundInfluence().ToString();
        //ui and sound
        if (economics.Deposit < 20) { depositText.color = Color.red; SoundManager(4); }
		else depositText.color = new Color (0.49f, 0.49f, 0.49f);
        if (economics.Deposit <= 0 && gameOver == false) GameOver();
    }

    //mostlyUI
    public void OutOfBounds (Economics.EcoState ecoState)
	{
		if (ecoState == Economics.EcoState.lower)
		{	
			depositBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
			SoundManager(4);
		} 
		else if (ecoState == Economics.EcoState.upper) 
		{
            influenceBar.GetComponent<Image>().CrossFadeColor(Color.red, 0.3f, false, false);
			SoundManager(4);
		}			
        else 
		{
            depositBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
            influenceBar.GetComponent<Image>().CrossFadeColor(Color.white, 0.3f, false, false);
		}
	}

	public void SetQuantity (int setQuantity)
	{
			if (positionOpen == false)
			{
                economics.SetQuantity(setQuantity);
			}
        //ui
		quantityText.text = economics.Quantity.ToString();
		SoundManager(3);
	}

	public void OpenBuyPosition ()
	{
		if (positionOpen == false) 
		{
            buying = true;
            PositionManager(true,economics.CurrentPrice, economics.Quantity); //ui?
            //ui
            openPositionPricePanel.localPosition = panelY;
		}
		else 
		{
            buying = true;
            PositionManager(false, economics.CurrentPrice, 0); //ui?
            //ui
            openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0); //ui
			if (economics.Profit > 0) SoundManager(1);
			else  SoundManager(2);  
		}
	}

    //ui?
	public void OpenSellPosition ()
	{
		if (economics.PositionOpen == false) 
		{
            buying = false;
            PositionManager(true,  economics.CurrentPrice, economics.Quantity);

            openPositionPricePanel.localPosition = panelY;
			Debug.Log(panelY);
			SoundManager(0);
		}
		else 
		{
            buying = false;
            PositionManager(false, economics.CurrentPrice, 0);

            openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0);
			if (economics.Profit > 0) SoundManager(1);
			else  SoundManager(2); 
		}
	}
    // Здесь что-то страшное происходит Надо пересмотреть метод
	public void PositionManager (bool open, float price, int quantity)
	{
		if ((price*quantity) <= economics.Deposit)
		{
            //Переменные == true? Исправить
			if (open == true)
			{
                economics.OpenPrice = price;
				economics.stock = economics.OpenPrice*quantity;
                economics.Deposit -=(economics.OpenPrice*quantity); 
				positionOpen = true; 

				openPositionNumText.text = economics.OpenPrice.ToString(); //ui
				SoundManager(0); //Что? о.о
				SoundManager(0);
			}
			if (open == false)
			{
                economics.OpenPrice = price;
                economics.Deposit += (economics.Profit+economics.stock);
				positionOpen = false;

                openPositionNumText.text = economics.OpenPrice.ToString(); //ui
                ProgressStorage(0, economics.Profit);
			}
		}
		else 
		{
			Debug.Log("No money, no honey!");
            //ui
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
		economics.Profit = 0;
		ScoreCounter (overallCycles, topPositionProfit, topSessionProfit);
		positionOpen = false;
		// experience += finalScore;
        //ui
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
