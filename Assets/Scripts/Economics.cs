using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economics : MonoBehaviour, IPauseble {

    public float influenceMax;                  //Максимальное количество очков влияния //public
    public float influenceRiseSpeed;            //Скорость восстановления очков влияния //public
    public float influenceFallSpeed;            //Скорость растраты очков влияния       //public
    public float influence;                     //Текущее количество очков влияния //public
    //[SerializeField] float deposit;             //Объём депозита (денег для торговли)
    public float Deposit { get; set; }
    public float RndDeposit { get { return Mathf.Round((Deposit * 100.0f) / 100.0f); } }
    [SerializeField] float initialPrice;        //Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
    [SerializeField] float currentPrice;        //Текущая цена инструмента
    public float CurrentPrice { get { return currentPrice; } set { currentPrice = value; } }
    [SerializeField] float supportPrice;        //Значение цены уровня сопротивления (верхняя красная граница)
    public float SupportPrice { get { return supportPrice; } set { supportPrice = value; } }
    [SerializeField] float resistancePrice;     //Значение цены уровня поддержки (нижняя зелёная граница)
    public float ResistancePrice { get { return resistancePrice; } set { resistancePrice = value; } }

    public float OpenPrice { get; set; }           //Цена открытия позиции (начала сделки)
    [SerializeField] float closePrice;          //Цена закрытия позиции (конца сделки)
    public float profit { get; set; }              //Доход от сделки
    public int Quantity { get; set; }				//Количество товара торгуемого инстумента
    [SerializeField] float spread;              //Коэффициент разницы цен между ценой сопротивления и поддержки
    [SerializeField] float devaluation;         //Коэффициент обесценивания инструмента при падении ниже линии поддержки
    [SerializeField] float comission;           //Размер комиссии, отчисляемой брокеру при открытии позиции
    public float Comission { get { return comission; } set { comission = value; } }
    public float stock;
    float newSpread;
    int newQuantity;
    public bool PositionOpen { get; set; }
    public bool PositionClose { get; set; }


    public Vector3 PanelY { get; set; }
    float pauseInfluenceRiseSpeed;
    float pauseInfluenceFallSpeed;

    public void StartEconomics()
    {
        currentPrice = initialPrice;
        Quantity = 1;
        SetPrices();
    }

    public void Pause()
    {
        pauseInfluenceRiseSpeed = influenceRiseSpeed;
        pauseInfluenceFallSpeed = influenceFallSpeed;
        influenceRiseSpeed = 0f;
        influenceFallSpeed = 0f;
    }

    public void Resume()
    {
        influenceFallSpeed = pauseInfluenceFallSpeed;
        influenceRiseSpeed = pauseInfluenceRiseSpeed;
    }

    public void SetQuantity(int setQuantity)
    {
        newQuantity = Quantity + setQuantity; //+=? What?
        if (newQuantity > 1)
        {
            Quantity = newQuantity;
        }
        else Quantity = 1;
    }

    public void SetPrices()
    {
        initialPrice = Mathf.Round(initialPrice * 100f) / 100f;
        resistancePrice = Mathf.Round((initialPrice * spread) * 100f) / 100f;
        supportPrice = Mathf.Round((initialPrice / spread) * 100f) / 100f;
        newSpread = resistancePrice - supportPrice;
    }

    public void Devaluation(float deltaTime)
    {
        Deposit -= Mathf.Lerp(0, devaluation, deltaTime * 5.0f);
    }

    public void MaxInfluenceDevaluation(float deltaTime)
    {
        influenceMax -= Mathf.Lerp(0, devaluation, deltaTime * 5.0f);
    }

    public float RoundInfluence()
    {
        return Mathf.Round((influence * 100.0f) / 100.0f);
    }

    public void UpdateCurrentPrice(CoinController coin)
    {
        currentPrice = Mathf.Round((initialPrice + coin.posY * (1 / newSpread)) * 100.0f) / 100.0f;
    }

    public void ProfitDepositMath(GameManager.GS gameState, bool positionOpen, bool buying)
    {
        //profit + deposit math
        if (positionOpen == true && buying == true)
        {
            profit = Mathf.Round(((currentPrice - OpenPrice) * Quantity) * 100.0f) / 100.0f;
            if (gameState == GameManager.GS.Play) Deposit -= comission;
            //Debug.Log("OP: " + economics.OpenPrice + " | Current Price: " + economics.CurrentPrice + " | Profit: " + economics.profit);
        }
        else if (positionOpen == true && buying == false)
        {
            profit = Mathf.Round(((OpenPrice - currentPrice) * Quantity) * 100.0f) / 100.0f;
            if (gameState == GameManager.GS.Play) Deposit -= comission;
            //Debug.Log("OP: " + economics.OpenPrice + " | Current Price: " + economics.CurrentPrice + " | Profit: " + economics.profit);
        }
        else profit = 0;
        //float oldDeposit = deposit;
        Deposit = Mathf.Round(Deposit * 100) / 100;
    }
}
