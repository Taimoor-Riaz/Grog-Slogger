using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    [SerializeField] private Image emptyImage;
    [SerializeField] private Image filledImage;

    public void SetState(bool isAlive)
    {
        emptyImage.gameObject.SetActive(!isAlive);
        filledImage.gameObject.SetActive(isAlive);
    }
}
