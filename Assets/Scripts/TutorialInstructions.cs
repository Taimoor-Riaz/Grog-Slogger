using UnityEngine;
using UnityEngine.UI;

public class TutorialInstructions : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private Text textComponent;
    void Awake()
    {
        Disable();
    }
    public void SetText(string text)
    {
        holder.SetActive(true);
        textComponent.text = text;
    }
    public void Disable()
    {
        holder.SetActive(false);
    }
}