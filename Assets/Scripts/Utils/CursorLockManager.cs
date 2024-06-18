using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorLockManager : MonoBehaviour
{
    private static CursorLockManager instance;
    public static CursorLockManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CursorLockManager>();
                instance.gameObject.name = "CursorLockManager";

                if (instance == null)
                {
                    GameObject go = new GameObject("CursorLockManager");
                    instance = go.AddComponent<CursorLockManager>();
                }
            }
            return instance;
        }
    }
    public bool IsLocked { get; private set; }

    private void Update()
    {
        /*
                if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        // If Mouse button is pressed, Lock Cursor
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Check if mouse is not on a UI element
            if (EventSystem.current != null)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    LockCursor();
                }
            }
            else
            {
                LockCursor();
            }
        }
         */
    }

    public static void ToggleLockState()
    {
        if (Instance == null)
        {
            return;
        }
        Instance.IsLocked = !Instance.IsLocked;
        if (Instance.IsLocked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }
    public static void LockCursor()
    {
        if (Instance == null)
        {
            return;
        }
        Instance.IsLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void UnlockCursor()
    {
        if (Instance == null)
        {
            return;
        }
        Instance.IsLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
