using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics {

    float time;
    float topPositionProfit;
    float topSessionProfit;

    public float TopPositionProfit { get { return topPositionProfit; } }
    public float Time { get { return time; } }
    public float TopSessionProfit { get { return topSessionProfit; } }

    public Statistics()
    {
        time = 0;
        topSessionProfit = 0;
        topPositionProfit = 0;
    }

    public void ProgressStorage(float time, float tPP)
    {
        this.time = time;
        if (tPP > topPositionProfit) topPositionProfit = tPP;
        topSessionProfit += tPP;
    }

    public float ScoreCounter()
    {
        return time + topPositionProfit + topSessionProfit;
    }
}
