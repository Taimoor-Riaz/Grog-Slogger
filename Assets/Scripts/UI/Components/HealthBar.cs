using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFiller;

    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    public void SetHealth(float health, float maxHealth)
    {
        float ratio = health / maxHealth;
        Color currentColor = Color.Lerp(endColor, startColor, ratio);

        healthFiller.fillAmount = ratio;
        healthFiller.color = currentColor;
    }
}
