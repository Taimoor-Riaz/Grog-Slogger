using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameUIController uiController;
    [SerializeField] private EnemiesManager enemiesManager;

    [Header("Audio")]
    [SerializeField] private AudioClip normalGameplayBackground;
    [SerializeField] private AudioClip shootingGameplayBackground;

    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public int MaxLives { get; private set; }
    [field: SerializeField] public int PointsForWave { get; private set; }

    public bool IsPaused { get; private set; }
    public int PlayerScore { get; private set; }
    public int CurrentLives { get; private set; }
    public int NextWaveNumber { get; private set; }

    public event Action<int> OnPlayerScoreUpdated;
    public event Action<int> OnLivesUpdated;

    void Awake()
    {
        CurrentLives = MaxLives;
        IsPaused = false;
    }

    void Start()
    {
        Player.OnCoinCollected += Player_OnCoinCollected;
        Player.OnPlayerDeath += OnPlayerDeath;

        enemiesManager.OnWaveStart += EnemiesManager_OnWaveStart;
        enemiesManager.OnWaveCompleted += EnemiesManager_OnWaveCompleted;

        SoundManager.Instance.SetBackgroundMusic(normalGameplayBackground);

        CursorLockManager.LockCursor();
        Time.timeScale = 1f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1f;
                uiController.HideGamePauseScreen();
            }
            else
            {
                IsPaused = true;
                Time.timeScale = 0f;
                uiController.ShowGamePauseScreen(() =>
                {
                    Time.timeScale = 1f;
                    GoToMainMenu();
                }, () =>
                {
                    IsPaused = false;
                    Time.timeScale = 1f;
                });
            }
        }
    }
    private void EnemiesManager_OnWaveCompleted()
    {
        SoundManager.Instance.SetBackgroundMusic(normalGameplayBackground);
    }

    private void EnemiesManager_OnWaveStart()
    {
        SoundManager.Instance.SetBackgroundMusic(shootingGameplayBackground);
    }

    private void OnPlayerDeath()
    {
        if (CurrentLives > 0)
        {
            CurrentLives--;
            Debug.Log("Lived Updated");
            OnLivesUpdated?.Invoke(CurrentLives);

            if (CurrentLives <= 0)
            {
                Time.timeScale = 0f;
                CursorLockManager.UnlockCursor();
                uiController.ShowGameOverScreen(GoToMainMenu, RestartGame);
            }
            else
            {
                Debug.Log("Player Revived");
                Player.Revive();
            }
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("Loading");
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Player_OnCoinCollected(object sender, OnCoinCollectedEventArgs e)
    {
        PlayerScore += e.Coin.Amount;
        OnPlayerScoreUpdated?.Invoke(PlayerScore);

        int targetPoints = PointsForWave * NextWaveNumber;

        if (PlayerScore >= targetPoints)
        {
            if (targetPoints != 0)
            {
                int enemiesAmount = NextWaveNumber;
                if (enemiesAmount > 5)
                {
                    enemiesAmount = 5;
                }
                enemiesManager.SpawnWave(enemiesAmount);
            }
            NextWaveNumber++;
        }
    }
}
public class Wave
{
    [SerializeField]
    [Range(0, 5)]
    public int EnemiesAmount;

    [SerializeField]
    public int ScoreTarget;
}