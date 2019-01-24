using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public bool firstRun;
    public float gameSpeed;             //Общая скорость игры //public
    public int cycleCounter;            //Глобальный счётчик длительности игровой сессии //public
    Timer timer;
    float gameSpeedPause;
    GS gameState;
    public GS GameState { get { return gameState; } }

    [SerializeField] CoinController coin;
    [SerializeField] Economics economics;
    [SerializeField] UIManager uiManager;
    Statistics statistics;

    [SerializeField] float topPositionProfit;       //Самый большой доход по сделке
    [SerializeField] float topSessionProfit;        //Самый большой доход за сессию 
    [SerializeField] float time;           //Длительность игровой сессии
    [SerializeField] float experience;          //Накопленный опыт
    [SerializeField] float finalScore;			//Количество очков опыта

    ////logic
    //bool positionOpen;
    //bool buying;

    //Audio
    [SerializeField] AudioClip[] audioClip;
    AudioSource audioPlayer;

    //UI
    [SerializeField] RectTransform mainTutorialPanel; 


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
		PreRun ();        
	}

	void PreRun()
	{
		if (firstRun == true)
        {
            mainTutorialPanel.gameObject.SetActive(true);
            mainTutorialPanel.GetComponent<Tutorial>().RunTutorial(0);
            gameState = GS.Pause;
        }
	}

	public void StartGame()
	{
        statistics = new Statistics();
        uiManager.InitializeUI();
        economics.StartEconomics();
        uiManager.SetPricesUI(economics.GetStatus());
        coin.StartCoin(economics.influenceMax);
        timer.StartTimer();
        gameState = GS.Play;
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
            statistics.ProgressStorage(timer.RoundedTimeSecs(),0);
            uiManager.UpdateUI(economics.GetStatus());
        }
        //ui and sound
        if (economics.Deposit < 20) SoundManager(4);
        else if (economics.Deposit <= 0 && gameState != GS.Over) GameOver();
    }

    //mostlyUI
    public void OutOfBounds (Economics.EcoState ecoState)
	{
		if (ecoState == Economics.EcoState.lower)
		{
            uiManager.DepositGoRed();
			SoundManager(4);
		} 
		else if (ecoState == Economics.EcoState.upper) 
		{
            uiManager.InfluenceGoRed();
            SoundManager(4);
		}			
        else 
		{
            uiManager.DepositGoWhite();
            uiManager.InfluenceGoWhite();
        }
	}

	public void SetQuantity (bool increase)
	{
        print("Nya");
        economics.SetQuantity(increase);
        uiManager.SetQuantity(economics.GetStatus());
		SoundManager(3);
	}

	public void OpenBuyPosition ()
	{
        if (economics.OpenBuyPosition()) 
		{
            uiManager.OpenBuyPosition(economics.GetStatus());
            SoundManager(0);
        }
	}

    //ui?
	public void OpenSellPosition ()
	{
        if (economics.OpenSellPosition()) 
		{
            SoundManager(0);
        }
	}

    public void ClosePosition()
    {
        if (economics.ClosePosition())
        {

            statistics.ProgressStorage(0, economics.Profit);
            uiManager.ClosePosition(economics.GetStatus());
            if (economics.Profit > 0) SoundManager(1);
            else SoundManager(2);
        }
    }

	public void SoundManager(int clip)
	{
		audioPlayer.clip = audioClip[clip]; 
		audioPlayer.Play();
	}

	public void PauseButton()
	{
		if (gameState == GS.Play)
		{
            PauseGame();
            uiManager.UISetPause(true);
		}
		else if (gameState == GS.Pause)
		{
            ResumeGame();
            uiManager.UISetPause(false);
		}
	}

    public void PauseGame()
    {
        gameState = GS.Pause;
        timer.PauseTimer();
    }
    public void ResumeGame()
    {
        gameState = GS.Play;
        timer.ResumeTimer();
    }

    public void RestartGame()
	{
		SceneManager.LoadScene (1);
	}

	public void GameOver ()
	{
        gameState = GS.Over;
        timer.PauseTimer();
        SoundManager(5);
        uiManager.GameOverUI(statistics);
		//economics.PositionOpen = false;
		// experience += finalScore;

        Debug.Log("OVERALL SCORE:  " + finalScore + "  || Cycles: " + time + "  || Timer: " + timer.RoundedTimeSecs()
            + " || TPP: " + topPositionProfit + " || TSP: " + topSessionProfit + "\n EXPERIENCE: " + experience);
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene (0);
	}
}
