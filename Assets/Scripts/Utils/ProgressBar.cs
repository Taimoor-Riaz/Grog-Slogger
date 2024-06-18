using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fillImage;

    private CameraController cameraController;

    void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
    }
    void Update()
    {
        if (cameraController != null)
        {
            transform.rotation = cameraController.Camera.transform.rotation;
        }
    }

    public void Activate()
    {
        canvas.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        canvas.gameObject.SetActive(false);
    }
    public void SetProgress(float amount)
    {
        fillImage.fillAmount = amount;
    }
    public void SetProgress(float current, float total)
    {
        SetProgress(current / total);
    }
}
