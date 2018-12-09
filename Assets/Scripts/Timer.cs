using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    float timer;
    bool timerIsActive;

    public void StartTimer()
    {
        timer = 0;
        timerIsActive = true;
    }

    public void ResumeTimer()
    {
        timerIsActive = true;
    }

    public void PauseTimer()
    {
        timerIsActive = false;
    }

    public void StopTimer()
    {
        timer = 0;
        timerIsActive = false;
    }
    public int RoundedTimeSecs()
    {
        return Mathf.RoundToInt(timer);
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timerIsActive) timer += Time.deltaTime;	
	}
}
