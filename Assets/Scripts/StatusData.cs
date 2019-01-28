using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData
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

    public StatusData(float deposit, float currentPrice, float supportPrice, float resistancePrice, float profit, float priceToDeltaPos, float influence,
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
