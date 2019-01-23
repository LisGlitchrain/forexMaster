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

    ////logic
    //bool positionOpen;
    //bool buying;

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
    [SerializeField] GameObject panelBuySellBtns;
    [SerializeField] GameObject panelClosePosBtn;

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
        SetPricesUI();
        coin.StartCoin(economics.influenceMax);

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
            economics.ProfitMath(gameState);
            economics.Devaluation(Time.deltaTime);
            economics.InfluenceDevaluation(Time.deltaTime);
            if (economics.GetEconomicChanged())
            {
                OutOfBounds(economics.EcoStatus);
            }
            //stats
            if (cycleCounter == 60)
            {
                ProgressStorage(1, 0);
                cycleCounter = 0;
            }
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
        economics.SetQuantity(setQuantity);
        //ui
		quantityText.text = economics.Quantity.ToString();
		SoundManager(3);
	}

	public void OpenBuyPosition ()
	{
        if (economics.OpenBuyPosition()) 
		{
            openPositionPricePanel.localPosition = panelY;
            panelBuySellBtns.SetActive(false);
            panelClosePosBtn.SetActive(true);

            openPositionNumText.text = economics.OpenPrice.ToString(); //ui
            SoundManager(0); //Что? о.о
            SoundManager(0);
        }
	}

    //ui?
	public void OpenSellPosition ()
	{
        if (economics.OpenSellPosition()) 
		{
            //ui
            openPositionPricePanel.localPosition = panelY;
			SoundManager(0);
            panelBuySellBtns.SetActive(false);
            panelClosePosBtn.SetActive(true);

            openPositionNumText.text = economics.OpenPrice.ToString(); //ui
            SoundManager(0); //Что? о.о
            SoundManager(0);
        }
	}

    public void ClosePosition()
    {
        if (economics.ClosePosition())
        {
            //ui
            openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0); //ui
            if (economics.Profit > 0) SoundManager(1);
            else SoundManager(2);
            panelBuySellBtns.SetActive(true);
            panelClosePosBtn.SetActive(false);
            openPositionNumText.text = economics.OpenPrice.ToString(); //ui
            ProgressStorage(0, economics.Profit);
        }
    }

	public void SoundManager(int clip)
	{
		audioPlayer.clip = audioClip[clip]; 
		audioPlayer.Play();
	}

    //ui
    public void SetPricesUI ()
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
	}

	public void ResumeGame()
	{
		gameSpeed = gameSpeedPause;
		gameState = GS.Play;
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
		PauseGame(0);
		economics.Comission = 0;
		if (economics.PositionOpen == true) economics.Deposit = 0;
		economics.Profit = 0;
		ScoreCounter (overallCycles, topPositionProfit, topSessionProfit);
		economics.PositionOpen = false;
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
