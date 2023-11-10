using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerLog : MonoBehaviour
{
    private void Start()
    {
        Timer timer = GetComponent<Timer>();
        timer.StartTimer();
    }
    public void TimerStarted(Timer t)
    {
        Debug.Log("Timer Started! " + t.Duration + " seconds");
    }
    public void TimerExpired(Timer t)
    {
        Debug.Log("Timer Expired :(");
    }
    public void TimerTick(Timer t)
    {
        Debug.Log("Timer Started! " + t.TimeLeft() + " seconds");
    }
}
