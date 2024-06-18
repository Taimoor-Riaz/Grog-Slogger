using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BugsBunny.Utilities.Timer;

public class DamageVisualizer : MonoBehaviour
{
    [SerializeField] private float Time = 0.5f;
    [SerializeField] private Image damageImage;
    [SerializeField] private Vector2 minMaxAlpha;

    private Timer runningTimer;

    public void VisualizeDamage()
    {
        if (runningTimer != null)
        {
            runningTimer.Stop();
        }

        damageImage.gameObject.SetActive(true);
        runningTimer = new Timer(Time, () =>
        {
            damageImage.gameObject.SetActive(false);
        });
        runningTimer.SetOwner(gameObject);
        runningTimer.AddOnUpdateAction((passed) =>
        {
            if (passed
            < Time / 2f)
            {
                damageImage.color = new Color(1, 1, 1, Mathf.Lerp(minMaxAlpha.x, minMaxAlpha.y, passed / Time / 2));
            }
            else
            {
                damageImage.color = new Color(1, 1, 1, Mathf.Lerp(minMaxAlpha.y, minMaxAlpha.x, (passed - Time / 2f) / Time / 2));
            }
        });
        TimersManager.Add(runningTimer);
    }
}
