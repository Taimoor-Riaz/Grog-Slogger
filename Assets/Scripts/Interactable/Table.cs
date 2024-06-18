using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, BaseInteractable
{
    private List<Chair> chairs = new List<Chair>();

    private void Awake()
    {
        chairs.AddRange(GetComponentsInChildren<Chair>());
    }

    public void OnEnter(Player player)
    {
    }

    public void OnExit(Player player)
    {
    }
    public void OnStay(Player player)
    {
    }
    public void OnInteractButton(Player player)
    {
        List<Chair> chairsToServe = new List<Chair>();

        for (int i = 0; i < chairs.Count; i++)
        {
            if (chairs[i].IsAcquired && (!chairs[i].HasMug) && (player.Mug.IsFilled))
            {
                chairsToServe.Add(chairs[i]);
            }
        }
        if (chairsToServe.Count == 0) { return; }

        Chair targetChair = chairsToServe[0];

        if (chairsToServe.Count > 1)
        {
            for (int i = 1; i < chairsToServe.Count; i++)
            {
                if (chairsToServe[i].BookingTime < targetChair.BookingTime)
                {
                    targetChair = chairsToServe[i];
                }
            }
        }
        targetChair.PlaceMug(player);
    }

    public void OnInteractButtonDown(Player player)
    {
    }

    public void OnInteractButtonUp(Player player)
    {
    }
}
