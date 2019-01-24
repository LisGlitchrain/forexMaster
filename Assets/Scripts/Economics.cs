using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economics : MonoBehaviour {

    public float influenceMax;                  //Максимальное количество очков влияния //public
    public float influenceRiseSpeed;            //Скорость восстановления очков влияния //public
    public float influenceFallSpeed;            //Скорость растраты очков влияния       //public
    public float influence;                     //Текущее количество очков влияния //public
    public int Quantity { get; set; }				//Количество товара торгуемого инстумента
    public float OpenPrice { get; set; }           //Цена открытия позиции (начала сделки)
    public float Profit { get ; set; }              //Доход от сделки

    [SerializeField] float initialPrice;        //Начальная для игровой сессии цена инструмента (тут и далее: инструмент это пара товаров (Евро/Доллар, например))
    [SerializeField] float currentPrice;        //Текущая цена инструмента
    [SerializeField] float supportPrice;        //Значение цены уровня сопротивления (верхняя красная граница)
    [SerializeField] float resistancePrice;     //Значение цены уровня поддержки (нижняя зелёная граница)
    [SerializeField] float closePrice;          //Цена закрытия позиции (конца сделки)
    [SerializeField] float spread;              //Коэффициент разницы цен между ценой сопротивления и поддержки
    [SerializeField] float devaluation;         //Коэффициент обесценивания инструмента при падении ниже линии поддержки
    [SerializeField] float influenceDevaluation;
    [SerializeField] float comission;           //Размер комиссии, отчисляемой брокеру при открытии позиции
    [SerializeField] float currentPriceRiseSpeed;
    [SerializeField] float currentPriceFallSpeed;
    EcoState newEcoState = EcoState.middle;
    EcoState ecoState = EcoState.middle;
    float stock;
    float newSpread;
    float pauseInfluenceRiseSpeed;
    float pauseInfluenceFallSpeed;

    public bool PositionOpen { get; set; }
    public bool PositionClose { get; set; }
    public bool Buying { get; set; }
    public float Deposit { get; set; }
    public float RndDeposit { get { return Rounder.RoundToHundredth(Deposit); } }
    public float CurrentPrice { get { return currentPrice; } set { currentPrice = value; } }
    public float PriceToDeltaPos { get { return (currentPrice - initialPrice)*3.5f; } }
    public float SupportPrice { get { return supportPrice; } set { supportPrice = value; } }
    public float ResistancePrice { get { return resistancePrice; } set { resistancePrice = value; } }
    public EcoState EcoStatus { get { return ecoState; } }

    public class Status
    {
        float deposit;
        float currentPrice;
        float supportPrice;
        float resistancePrice;
        float profit;
        float priceToDeltaPos;
        float influence;
        float influenceMax;
        int quantity;
        float openPrice;

        public float Deposit { get { return deposit; } }
        public float RndDeposit { get { return Rounder.RoundToHundredth(deposit); } }
        public float CurrentPrice { get { return currentPrice; } }
        public float SupportPrice { get { return supportPrice; } }
        public float ResistancePrice { get { return resistancePrice; } }
        public float Profit { get { return profit; } }
        public float Influence { get { return influence; } }
        public float InfluenceMax { get { return influenceMax; } }
        public float PriceToDeltaPos { get { return priceToDeltaPos; } }
        public float Quantity { get { return quantity; } }
        public float OpenPrice { get { return openPrice; } }

        public Status (float deposit, float currentPrice, float supportPrice, float resistancePrice, float profit, float priceToDeltaPos, float influence, 
                        float influenceMax, int quantity, float openPrice)
        {
            this.deposit = deposit;
            this.currentPrice = currentPrice;
            this.supportPrice = supportPrice;
            this.resistancePrice = resistancePrice;
            this.profit = profit;
            this.priceToDeltaPos = priceToDeltaPos;
            this.influence = influence;
            this.influenceMax = influenceMax;
            this.priceToDeltaPos = priceToDeltaPos;
            this.quantity = quantity;
            this.openPrice = openPrice;
        }
    }

    public enum EcoState
    {
        upper,
        middle,
        lower
    }

    public void StartEconomics()
    {
        SetPrices();
        currentPrice = initialPrice;
        influence = 2f;
        Quantity = 1;
        Deposit = 200f;
        PositionOpen = false;
    }

    //public void Pause()
    //{
    //    pauseInfluenceRiseSpeed = influenceRiseSpeed;
    //    pauseInfluenceFallSpeed = influenceFallSpeed;
    //    influenceRiseSpeed = 0f;
    //    influenceFallSpeed = 0f;
    //}

    //public void Resume()
    //{
    //    influenceFallSpeed = pauseInfluenceFallSpeed;
    //    influenceRiseSpeed = pauseInfluenceRiseSpeed;
    //}

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
        print("Quantity " + Quantity);
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

    //ui?
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

    public bool ClosePosition()
    {
        if (PositionOpen)
        {
            OpenPrice = CurrentPrice;
            Deposit += (Profit + stock);
            PositionOpen = false;
            return true;
        }
        else return false;

    }

    public void SetPrices()
    {
        initialPrice = Rounder.RoundToHundredth(initialPrice);
        resistancePrice = Rounder.RoundToHundredth(initialPrice * spread);
        supportPrice = Rounder.RoundToHundredth(initialPrice / spread);
        newSpread = resistancePrice - supportPrice;
    }

    public void Devaluation(float deltaTime)
    {
        if (ecoState == EcoState.lower)
            Deposit -= devaluation * deltaTime;
    }

    public void InfluenceDevaluation(float deltaTime)
    {
        if (ecoState == EcoState.upper)
            influence -= influenceDevaluation * deltaTime;
    }

    public float RoundInfluence()
    {
        return Rounder.RoundToHundredth(influence);
    }

    public void UpdateCurrentPrice(float deltatime, int touchCount, bool lmbPressed)
    {
        //АААААА!!!11111 coin.posY Здесь использовать нельзя ! обратная зависимость
        if ((touchCount > 0 || lmbPressed) && currentPrice < resistancePrice+ 0.3) //Нужно подниматься чуть выше границы
        {
            currentPrice = Rounder.RoundToHundredth(currentPrice + currentPriceRiseSpeed * deltatime); //coin.posY * (1 / newSpread)=
        }
        else if (currentPrice > supportPrice-0.3) //Нужно опускаться чуть ниже границы
        {
            currentPrice = Rounder.RoundToHundredth(currentPrice - currentPriceFallSpeed * deltatime); //coin.posY * (1 / newSpread)
        }
    }

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

    public void ProfitMath(GameManager.GS gameState)
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

    public Status GetStatus()
    {
        return new Status(RndDeposit,CurrentPrice,SupportPrice,ResistancePrice, Profit,PriceToDeltaPos, RoundInfluence(), influenceMax, Quantity, OpenPrice);
    }

}
