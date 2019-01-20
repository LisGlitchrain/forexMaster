using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rounder
{
    public static float RoundToHundredth(float value)
    {
        return Mathf.Round(value * 100)/100;
    }

}
