using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPItems : MonoBehaviour
{
    public Gun Gun { get; private set; }

    void Awake()
    {
        Gun = GetComponentInChildren<Gun>();
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
