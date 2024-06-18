using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private string loadingSceneName;

    [SerializeField] private AudioClip backgroundMusic;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        SoundManager.Instance.SetBackgroundMusic(backgroundMusic);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(loadingSceneName);
    }
}
