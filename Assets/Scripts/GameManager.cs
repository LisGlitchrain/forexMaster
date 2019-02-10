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
    float currentDeltaTime;

    PlayerData playerData;

    [SerializeField] CoinController coin;
    [SerializeField] Economics economics;
    [SerializeField] UIManager uiManager;
    Statistics statistics;
    [SerializeField] Background background;
    [SerializeField] ObstacleManager obstacleManager;
    [SerializeField] TrendLine trendLine;

    [SerializeField] float topPositionProfit;       //Самый большой доход по сделке
    [SerializeField] float topSessionProfit;        //Самый большой доход за сессию 
    [SerializeField] float time;           //Длительность игровой сессии
    [SerializeField] float experience;          //Накопленный опыт
    [SerializeField] float finalScore;			//Количество очков опыта


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

            playerData = new PlayerData();
            playerData.deposit = 200f;
            playerData.quantity = 1;
            PlayerDataSaverLoader.SavePlayerData(playerData);
        }
	}

	public void StartGame()
	{
        playerData = PlayerDataSaverLoader.LoadPlayerData();
        statistics = new Statistics();
        uiManager.InitializeUI();
        economics.StartEconomics(playerData);
        uiManager.SetQuantity(economics.GetStatus());
        uiManager.SetPricesUI(economics.GetStatus());
        coin.StartCoin(economics.InfluenceMax);
        timer.StartTimer();
        gameState = GS.Play;
    }

	void Update() 
	{
        currentDeltaTime = Time.deltaTime;
        if (gameState == GS.Play)
        {
            coin.CoinUpdate(currentDeltaTime, gameSpeed, economics.PriceToDeltaPos);
            economics.EcoUpdate(currentDeltaTime, Input.touchCount, Input.GetMouseButton(0));
            if (economics.GetEconomicChanged())
            {
                OutOfBounds(economics.EcoStatus);
            }
            //stats
            statistics.ProgressStorage(timer.RoundedTimeSecs(),0);
            uiManager.UpdateUI(economics.GetStatus());
            background.BackgroundUpdate(currentDeltaTime);
            obstacleManager.UpdateIt(currentDeltaTime);
        }
        trendLine.UpdateLine(coin);
        //sound and logic
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
            uiManager.OpenSellPosition(economics.GetStatus());
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
        trendLine.Pause();
    }
    public void ResumeGame()
    {
        gameState = GS.Play;
        timer.ResumeTimer();
        trendLine.Resume();
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
        PlayerDataSaverLoader.SavePlayerData(economics.GetPlayerData());
	}

	public void MainMenu ()
	{
		SceneManager.LoadScene (0);
	}
}
