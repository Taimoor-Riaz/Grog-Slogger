using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 1f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
