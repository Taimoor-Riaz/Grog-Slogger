using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePausedPanel : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    public void Show(UnityAction onMainMenuClicked, UnityAction onResumeClicked)
    {
        mainMenuButton.onClick.AddListener(() => { onMainMenuClicked?.Invoke(); });
        resumeButton.onClick.AddListener(() => { onResumeClicked?.Invoke(); });

        gameObject.SetActive(true);
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }
}
