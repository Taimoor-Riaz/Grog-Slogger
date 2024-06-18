using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    public void Show(UnityAction onMainMenuClicked, UnityAction OnRestartClicked)
    {
        mainMenuButton.onClick.AddListener(() => { onMainMenuClicked?.Invoke(); });
        restartButton.onClick.AddListener(() => { OnRestartClicked?.Invoke(); });

        gameObject.SetActive(true);
    }
}
