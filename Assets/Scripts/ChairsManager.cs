using System;
using UnityEngine;

public class ChairsManager : MonoBehaviour
{
    [SerializeField] private Chair[] chairs;

    public Chair GetRandomTable()
    {
        for (int i = 0; i < chairs.Length; i++)
        {
            if (!chairs[i].IsAcquired)
            {
                return chairs[i];
            }
        }

        return null;
    }
    [ContextMenu("Get Child Chairs")]
    private void GetChildChairs()
    {
        chairs = GetComponentsInChildren<Chair>();
    }
}
