using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BaseInteractable
{
    void OnInteractButtonDown(Player player);
    void OnInteractButtonUp(Player player);
    void OnInteractButton(Player player);
    void OnEnter(Player player);
    void OnExit(Player player);
    void OnStay(Player player);
}
