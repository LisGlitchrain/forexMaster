using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economics : MonoBehaviour {

    [SerializeField] float influenceMax;                  //Максимальное количество очков влияния //public
    [SerializeField] float influenceRiseSpeed;            //Скорость восстановления очков влияния //public
    [SerializeField] float influenceFallSpeed;            //Скорость растраты очков влияния       //public
    [SerializeField] float influence;                     //Текущее количество очков влияния //public
    public int Quantity { get; set; }				//Количество товара торгуемого инстумента
    float OpenPrice { get; set; }           //Цена открытия позиции (начала сделки)
    /// <summary>
    /// Current profit.
    /// </summary>
    public float Profit { get ; set; }              //Доход от сделки


    [SerializeField] float initialPrice;        //Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
    [SerializeField] float currentPrice;        //Текущая цена инструмента
    [SerializeField] float supportPrice;        //Значение цены уровня сопротивления (верхняя красная граница)
    [SerializeField] float resistancePrice;     //Значение цены уровня поддержки (нижняя зелёная граница)
    [SerializeField] float closePrice;          //Цена закрытия позиции (конца сделки)
    [SerializeField] float spread;              //Коэффициент разницы цен между ценой сопротивления и поддержки
    [SerializeField] float devaluation;         //Коэффициент обесценивания инструмента при падении ниже линии поддержки
    [SerializeField] float influenceDevaluation; //эт я сам не знаю, что такое 
    [SerializeField] float comission;           //Размер комиссии, отчисляемой брокеру при открытии позиции
    [SerializeField] float currentPriceRiseSpeed;
    [SerializeField] float currentPriceFallSpeed;

    [SerializeField] float influenceDecaluationDivider;
    EcoState newEcoState;
    EcoState ecoState;
    float stock;
    float newSpread;
    bool PositionOpen { get; set; }
    bool Buying { get; set; }
    /// <summary>
    /// Current player's deposit.
    /// </summary>
    public float Deposit { get; set; }
    float RndDeposit { get { return Rounder.RoundToHundredth(Deposit); } }
    float RoundInfluence { get { return Rounder.RoundToHundredth(influence); } }
    public float InfluenceMax { get { return influenceMax; } }
    float CurrentPrice { get { return currentPrice; } set { currentPrice = value; } }
    /// <summary>
    /// Translation of current price to coin's delta position
    /// </summary>
    public float PriceToDeltaPos { get { return (currentPrice - initialPrice)*3.5f; } }
    float SupportPrice { get { return supportPrice; } set { supportPrice = value; } }
    float ResistancePrice { get { return resistancePrice; } set { resistancePrice = value; } }
    /// <summary>
    /// Contain information if current price above, below or between support/resistance prices.
    /// </summary>
    public EcoState EcoStatus { get { return ecoState; } }

    /// <summary>
    /// Contains 3 states describing current price's pisition relative to suppors/resistance prices.
    /// </summary>
    public enum EcoState
    {
        upper,
        middle,
        lower
    }

    /// <summary>
    /// Initialize economics with initial params.
    /// </summary>
    public void StartEconomics(PlayerData playerData)
    {
        SetPrices();
        currentPrice = initialPrice;
        influence = 2f;
        Quantity = playerData.quantity;
        Deposit = playerData.deposit;
        PositionOpen = false;
        newEcoState = EcoState.middle;
        ecoState = EcoState.middle;
    }

    /// <summary>
    /// Increases/decreases Quantity. Validation included.
    /// </summary>
    /// <param name="increase">True increases quantity by one, false decreases.</param>
    public void SetQuantity(bool increase)
    {
        if (!PositionOpen)
        {
            if (increase)
            {
                var newQuantity = Quantity + 1; //+=? What?
                if (newQuantity >= 1)
                {
                    Quantity = newQuantity;
                }
            }
            else
            {
                var newQuantity = Quantity - 1; //+=? What?
                if (newQuantity >= 1)
                {
                    Quantity = newQuantity;
                }
            }
        }
    }

    public bool OpenBuyPosition()
    {
        Buying = true;
        if (PositionOpen == false && (CurrentPrice * Quantity) <= Deposit)
        {
            PositionOpen = true;
            OpenPrice = CurrentPrice;
            stock = OpenPrice * Quantity;
            Deposit -= (OpenPrice * Quantity);
            return true;
        }
        else return false;
    }

    
    public bool OpenSellPosition()
    {
        Buying = false;
        if (PositionOpen == false && (CurrentPrice * Quantity) <= Deposit)
        {
            PositionOpen = true;
            OpenPrice = CurrentPrice;
            stock = OpenPrice * Quantity;
            Deposit -= (OpenPrice * Quantity);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Closes any open position. Calculates and changes Deposit. Returns true if position is closed successfully; false if not.
    /// </summary>
    /// <returns>True is position is closed successfully.</returns>
    public bool ClosePosition()
    {
        if (PositionOpen)
        {
            //OpenPrice = CurrentPrice;
            Deposit += (Profit + stock);
            PositionOpen = false;
            return true;
        }
        else return false;

    }
    /// <summary>
    /// Initializes prices.
    /// </summary>
    void SetPrices()
    {
        initialPrice = Rounder.RoundToHundredth(initialPrice);
        resistancePrice = Rounder.RoundToHundredth(initialPrice * spread);
        supportPrice = Rounder.RoundToHundredth(initialPrice / spread);
        newSpread = resistancePrice - supportPrice;
    }
    /// <summary>
    /// Devaluates deposit according time and initialized devaluation value.
    /// </summary>
    /// <param name="deltaTime">Delta time</param>
    public void Devaluation(float deltaTime)
    {
        if (ecoState == EcoState.lower || ecoState == EcoState.upper)
            Deposit -= devaluation * deltaTime;
    }
    /// <summary>
    /// Devaluates currency influence according time and initialized devaluation value.
    /// </summary>
    /// <param name="deltaTime">Delta time</param>
    void InfluenceDevaluation(float deltaTime, int touchCount, bool lmbPressed)
    {
        if ((touchCount > 0 || lmbPressed) &&influence>0)
            influence -= influenceDevaluation * deltaTime;
        else if (influence < influenceMax)
            influence += influenceDevaluation * deltaTime / influenceDecaluationDivider;
    }

    /// <summary>
    /// Updates economics variables according time and player's actions.
    /// </summary>
    /// <param name="deltatime">delta time</param>
    /// <param name="touchCount">Counts of touches</param>
    /// <param name="lmbPressed">Is left mouse button pressed?</param>
    public void EcoUpdate(float deltatime, int touchCount, bool lmbPressed)
    {
        UpdateCurrentPrice(deltatime, touchCount, lmbPressed);
        ProfitMath();
        Devaluation(Time.deltaTime);
        InfluenceDevaluation(Time.deltaTime,touchCount, lmbPressed);
    }


    void UpdateCurrentPrice(float deltatime, int touchCount, bool lmbPressed)
    {
        if ((touchCount > 0 || lmbPressed) && currentPrice < resistancePrice+ 0.3 && influence>0) //Нужно подниматься чуть выше границы
        {
            currentPrice = Rounder.RoundToHundredth(currentPrice + currentPriceRiseSpeed * deltatime); //coin.posY * (1 / newSpread)=
        }
        else if (currentPrice > supportPrice-0.3) //Нужно опускаться чуть ниже границы
        {
            currentPrice = Rounder.RoundToHundredth(currentPrice - currentPriceFallSpeed * deltatime); //coin.posY * (1 / newSpread)
        }
    }

    /// <summary>
    /// Returns true if economics state is changed. (Current price crosses support/resistance prices.)
    /// </summary>
    /// <returns></returns>
    public bool GetEconomicChanged()
    {
        if (currentPrice> resistancePrice)
            newEcoState = EcoState.upper;
        else if(currentPrice < supportPrice)
            newEcoState = EcoState.lower;
        else
            newEcoState = EcoState.middle;
        if (ecoState != newEcoState)
        {
            ecoState = newEcoState;
            return true;
        }
        else
        {
            return false;
        }
    }

    void ProfitMath()
    {
        //profit + deposit math
        if (PositionOpen == true && Buying == true)
        {
            Profit = Rounder.RoundToHundredth((currentPrice - OpenPrice) * Quantity);
            //if (gameState == GameManager.GS.Play) Deposit -= comission;
            //Debug.Log("OP: " + economics.OpenPrice + " | Current Price: " + economics.CurrentPrice + " | Profit: " + economics.profit);
        }
        else if (PositionOpen == true && Buying == false)
        {
            Profit = Rounder.RoundToHundredth((OpenPrice - currentPrice) * Quantity);
            //if (gameState == GameManager.GS.Play) Deposit -= comission;
            //Debug.Log("OP: " + economics.OpenPrice + " | Current Price: " + economics.CurrentPrice + " | Profit: " + economics.profit);
        }
        else Profit = 0;
        //float oldDeposit = deposit;
        //Deposit = Mathf.Round(Deposit * 100) / 100;

    }

    /// <summary>
    /// Returns data-class with info for UI.
    /// </summary>
    /// <returns></returns>
    public StatusData GetStatus()
    {
        return new StatusData(RndDeposit,CurrentPrice,SupportPrice,ResistancePrice, Profit,PriceToDeltaPos, RoundInfluence, influenceMax, Quantity, OpenPrice);
    }

    public PlayerData GetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        playerData.deposit = Deposit;
        playerData.quantity = Quantity;
        return playerData;
    }
}
