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
    public int playerHealth;

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

    private string currentScene;
    private IEnumerator DamageCoroutineObject;


    private bool isPlayerDamageable = true;

    private void Awake()
    {

        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Remove if we have a title screen
    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        LoadScene(Level1);
        
        OnGameStart.Invoke();
        TogglePause();
        TogglePause();
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
        Debug.Log("Loser!!!");
        OnGameLoss.Invoke();
        ResetScene();
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

    // Call this to reset everything
    public void ResetScene()
    {
        LoadScene(currentScene);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentScene = sceneName;
    }

    public void HealPlayer()
    {
        Debug.Log("Healing player: " + playerHealth);
        playerHealth++;
        OnPlayerHeal.Invoke();
    }

    public void HurtPlayer()
    {
        if (isPlayerDamageable)
        {
            DamageCoroutineObject = DoDamageCooldown();
            StartCoroutine(DamageCoroutineObject);

            playerHealth--;
            OnPlayerHurt.Invoke();
            Debug.Log("Player health:" + playerHealth);

            if (playerHealth <= 0)
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
            //This is a nice juice thing, if we want a volume to appear on damage (like red at the sides of the screen)

            //damageVolume.weight = (playerDamageCooldown - i) / playerDamageCooldown;
            yield return null;
        }

        isPlayerDamageable = true;
    }
}
