using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 45f;

    public int Amount { get; private set; } = 1;

    public void SetAmount(int amount)
    {
        Amount = amount;
    }
    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
