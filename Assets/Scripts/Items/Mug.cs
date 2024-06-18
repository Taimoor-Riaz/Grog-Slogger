using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mug : MonoBehaviour
{
    [SerializeField] private float fillSpeed;

    private float fillAmount;

    public float FillAmount => fillAmount;
    public bool IsFilled => fillAmount >= 1;
    public bool IsActive => gameObject.activeSelf;

    public void Fill()
    {
        float fill = fillAmount;
        fill += (fillSpeed * Time.deltaTime);

        SetFillAmount(fill);
    }
    public void SetFillAmount(float amount)
    {
        fillAmount = amount;
        fillAmount = Mathf.Clamp01(fillAmount);
    }
    public void Serve()
    {
        Deactivate();
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        fillAmount = 0f;
    }
}
