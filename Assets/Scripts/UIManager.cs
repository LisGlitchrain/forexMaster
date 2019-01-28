using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    Vector3 panelY;
    [SerializeField] Scrollbar influenceBar; 
    [SerializeField] Scrollbar depositBar; 
    Text depositText; 
    Text influenceText; 
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
    [SerializeField] float fadeSpeed;

    //Может сделать один метод перерисовки UI, и изменять данные...Или добавить листенера?
    //Через публичные свойства так сделать? установить сеттер с вызовом перерисовки?
    public static class Colors
    {
        public static Color red = new Color(182, 0, 0);
        public static Color green = new Color(0, 182, 0);
        public static Color white = new Color(240, 240, 240);
        public static Color gray = new Color(0.49f, 0.49f, 0.49f);
    }

    public void InitializeUI()
    {
        gameOverPanel.gameObject.SetActive(false);
        gamePausedPanel.gameObject.SetActive(false);
        depositText = depositBar.GetComponentInChildren<Text>();
        influenceText = influenceBar.GetComponentInChildren<Text>();
        gameOverPanel.gameObject.SetActive(false); 
        gamePausedPanel.gameObject.SetActive(false); 
    }
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateUI(StatusData status)
    {
        influenceBar.size = status.Influence / status.InfluenceMax;
        influenceText.text = status.Influence.ToString();
        panelY = Camera.main.WorldToScreenPoint(new Vector3(-7.81f, status.PriceToDeltaPos - 3.05f, 0.0f)); //Maguc numbers
        currentPricePanel.localPosition = panelY;
        depositBar.size = status.Deposit / 1000 * 5;
        currentPriceText.text = status.CurrentPrice.ToString();
        depositText.text = status.RndDeposit.ToString();
        profitText.text = status.Profit.ToString();

        if (status.Profit > 0) profitText.color = Colors.green;
        else if (status.Profit < 0) profitText.color = Colors.red;
        else profitText.color = Colors.white;
        if (status.Deposit < 20) depositText.color = Colors.red;
        else depositText.color = Colors.gray;
    }

    void FinishStats()
    {

    }

    void BarFadeToColor(Scrollbar bar, Color color, float fadingSpeed)
    {
        depositBar.GetComponent<Image>().CrossFadeColor(color, fadingSpeed, false, false);
    }

    public void DepositGoRed()
    {
        BarFadeToColor(depositBar, Colors.red, fadeSpeed);
    }

    public void DepositGoWhite()
    {
        BarFadeToColor(depositBar, Colors.white, fadeSpeed);
    }

    public void InfluenceGoRed()
    {
        BarFadeToColor(influenceBar, Colors.red, fadeSpeed);
    }

    public void InfluenceGoWhite()
    {
        BarFadeToColor(influenceBar, Colors.white, fadeSpeed);
    }

    public void SetQuantity(StatusData status)
    {
        quantityText.text = status.Quantity.ToString();
    }

    public void OpenBuyPosition(StatusData status)
    {
        panelBuySellBtns.SetActive(false);
        panelClosePosBtn.SetActive(true);
        openPositionNumText.text = status.OpenPrice.ToString();
        openPositionPricePanel.localPosition = panelY;
    }

    public void OpenSellPosition(StatusData status)
    {
        openPositionPricePanel.localPosition = panelY;
        panelBuySellBtns.SetActive(false);
        panelClosePosBtn.SetActive(true);
        openPositionNumText.text = status.OpenPrice.ToString();
    }

    public void ClosePosition(StatusData status)
    {
        openPositionPricePanel.localPosition = new Vector3(-1000.0f, -1000.0f, 0); //ui //Maguc number
        panelBuySellBtns.SetActive(true);
        panelClosePosBtn.SetActive(false);
    }

    public void SetPricesUI(StatusData status)
    {
        resistancePriceText.text = status.ResistancePrice.ToString();
        supportPriceText.text = status.SupportPrice.ToString();
        currentPriceText.text = status.CurrentPrice.ToString();
    }

    public void UISetPause(bool isPaused)
    {
        if (isPaused) gamePausedPanel.gameObject.SetActive(true);
        else gamePausedPanel.gameObject.SetActive(false);
    }

    public void GameOverUI(Statistics stats)
    {
        gameOverPanel.gameObject.SetActive(true);
        //ui
        tPPText.text = stats.TopPositionProfit.ToString();
        tSPText.text = stats.TopSessionProfit.ToString();
        sLengthText.text = stats.Time.ToString();
        expText.text = stats.ScoreCounter().ToString();
        expBar.size = stats.ScoreCounter() / 100;
    }

}
