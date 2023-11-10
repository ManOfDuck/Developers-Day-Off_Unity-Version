using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer Configuration")]
    [SerializeField] float timerDuration;

    private bool timerStarted;
    private float endTime;


    //Timer Events
    [Header("Timer Events")]
    [SerializeField] private UnityEvent<Timer> OnTimerStarted;
    [SerializeField] private UnityEvent<Timer> OnTimerExpired;
    [SerializeField] private UnityEvent<Timer> OnTimerTick;


    //property to see if Timer has Started
    public bool TimerStarted { get { return timerStarted; } }
    public float Duration { get { return timerDuration; } set { timerDuration = value; } }


    public float TimeLeft()
    {
        float left = endTime - Time.time;
        if (left <= 0)
        {
            return 0;
        }
        else
            return left;
    }

    public void StartTimer()
    {
        if (!TimerStarted)
        {
            timerStarted = true;
            StartCoroutine(StartCountdown());
        }
    }

    protected IEnumerator StartCountdown()
    {
        endTime = Time.time + Duration;
        OnTimerStarted.Invoke(this);

        while (Time.time < endTime)
        {
            OnTimerTick.Invoke(this);

            yield return null;
        }

        OnTimerExpired.Invoke(this);
        //end of Couroruturitine
        yield return null;
    }

}
