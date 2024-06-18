using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private Text playerCoinsText;
    [SerializeField] private GameOverPanel gameOverScreen;
    [SerializeField] private GamePausedPanel gamePausedScreen;
    [SerializeField] private LifeUI[] livesUIs;

    private void OnEnable()
    {
        gameController.OnPlayerScoreUpdated += UpdatePlayerScoreText;
        gameController.Player.OnPlayerHealthChanged += UpdatePlayerHealth;
        gameController.OnLivesUpdated += UpdateLivesUI;

        UpdatePlayerScoreText(gameController.PlayerScore);
        UpdateLivesUI(gameController.MaxLives);
    }
    void OnDisable()
    {
        gameController.OnPlayerScoreUpdated -= UpdatePlayerScoreText;
        gameController.Player.OnPlayerHealthChanged -= UpdatePlayerHealth;
        gameController.OnLivesUpdated -= UpdateLivesUI;
    }
    private void UpdateLivesUI(int obj)
    {
        for (int i = 0; i < livesUIs.Length; i++)
        {
            livesUIs[i].SetState(i < obj);
        }
    }

    private void UpdatePlayerHealth()
    {
        playerHealthBar.SetHealth(gameController.Player.Health, 100f);
    }

    private void UpdatePlayerScoreText(int obj)
    {
        playerCoinsText.text = obj.ToString();
    }
    public void ShowGameOverScreen(UnityAction onMainMenuClicked, UnityAction OnRestartClicked)
    {
        CursorLockManager.UnlockCursor();
        gameOverScreen.Show(onMainMenuClicked, OnRestartClicked);
    }
    public void ShowGamePauseScreen(UnityAction onMainMenuClicked, UnityAction onResumeClicked)
    {
        CursorLockManager.UnlockCursor();
        gamePausedScreen.Show(onMainMenuClicked, () =>
        {
            onResumeClicked?.Invoke();
            HideGamePauseScreen();
        });
    }
    public void HideGamePauseScreen()
    {
        CursorLockManager.LockCursor();
        gamePausedScreen.Hide();
    }
}
