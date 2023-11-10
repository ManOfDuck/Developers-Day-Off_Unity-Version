using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//hi!

public class GameManager : MonoBehaviour
{
    [SerializeField] private string TitleScreen;
    [SerializeField] private string Level1;
    [SerializeField] private float playerDamageCooldown;
    [SerializeField] private UnityEngine.Rendering.Volume damageVolume;
    [SerializeField] public float initialMana;
    public float mana;


    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public enum GameState
    {
        Playing, Paused, GameOver
    }

    // this is a property with default getters/setters
    public GameState CurrentGameState { get; private set; }

    // events for the game being paused or resumed
    public UnityEvent OnGameStart;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameWin;
    public UnityEvent OnGameLoss;
    public UnityEvent OnGameQuit;
    public UnityEvent OnPlayerHurt;
    public UnityEvent OnPlayerHeal;

    private int playerHealth;


    private bool isPlayerDamageable = true;

    private void Awake()
    {
        mana = initialMana;

        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        playerHealth = 0;
    }

    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        SceneManager.LoadScene(Level1);
        OnGameStart.Invoke();
        TogglePause();
        TogglePause();

        for (int i = 0; i < playerHealth; i++)
        {
            HealPlayer();
        }
    }

    public void PauseGame()
    {
        CurrentGameState = GameState.Paused;
        OnGamePaused.Invoke();
    }

    public void ResumeGame()
    {
        CurrentGameState = GameState.Playing;
        OnGameResumed.Invoke();
    }

    public void WinGame()
    {
        CurrentGameState = GameState.GameOver;
        OnGameWin.Invoke();
    }

    public void LoseGame()
    {
        CurrentGameState = GameState.GameOver;
        OnGameLoss.Invoke();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(TitleScreen);
        OnGameQuit.Invoke();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void HealPlayer()
    {
        playerHealth++;
        OnPlayerHeal.Invoke();
    }

    public void HurtPlayer()
    {
        if (isPlayerDamageable)
        {
            StartCoroutine(DoDamageCooldown());

            playerHealth--;
            OnPlayerHurt.Invoke();

            if (playerHealth == 0)
            {
                LoseGame();
            }
        }
    }

    public void TogglePause()
    {
        if (CurrentGameState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (CurrentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }

    public float GetMana()
    {
        //return 0;
        return mana;
    }

    public void ResetMana()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResumeGame();
    }

    protected virtual IEnumerator DoDamageCooldown()
    {
        isPlayerDamageable = false;

        float i = 0;
        while (i < playerDamageCooldown)
        {
            i += Time.deltaTime;
            damageVolume.weight = (playerDamageCooldown - i) / playerDamageCooldown;
            yield return null;
        }

        isPlayerDamageable = true;
    }
}
