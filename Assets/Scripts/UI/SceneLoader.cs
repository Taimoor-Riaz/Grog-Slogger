using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private bool autoGetName;

    [SerializeField] private Image loadingFiller;

    private AsyncOperation loadingOperation;

    void Start()
    {
        if (autoGetName)
        {
            sceneName = PlayerPrefs.GetString("SceneToLoad");
        }
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
    }
    void Update()
    {
        if (loadingOperation != null)
        {
            loadingFiller.fillAmount = loadingOperation.progress;
        }
    }
}
