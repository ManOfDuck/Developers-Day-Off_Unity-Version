using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public enum GameState
    {
        Playing, Paused
    }

    public GameState CurrentGameState { get; private set; }

    //Events for game
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameWon;
    public UnityEvent OnGameOver;
    public UnityEvent OnGameStart;
    public UnityEvent OnCreditsShow;
    public UnityEvent OnQuit;

    private void Awake()
    {
        //is this the first time we've created this singleton?
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject); //kys
        }
    }

    public void ResumeGame()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResumed.Invoke();
    }
    public void PauseGame()
    {
       
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePaused.Invoke();
    }

    public void GameWon()
    {
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
        OnGameWon.Invoke();
    }

    public void GameOver()
    {
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
        OnGameOver.Invoke();
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        ResumeGame();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
