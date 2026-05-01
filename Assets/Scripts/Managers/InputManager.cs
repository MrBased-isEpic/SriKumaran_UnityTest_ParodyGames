using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region SINGLETON
    
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    #endregion
    
    #region Events

    public Action OnJumpPressed;
    
    #endregion 
    
    #region Properties

    public Vector2 MouseDelta => _mouseDelta;
    public bool isMouseLocked => !Cursor.visible;

    #endregion
    
    #region Interface

    public void LockMouse(bool lockMouse)
    {
        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ToggleMouseLock()
    {
        LockMouse(!isMouseLocked);
    }
    
    #endregion

    void Start()
    {
        LockMouse(true);
    }
    
    
    private Vector2 _mouseDelta;

    private void Update()
    {
        // _mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // //Debug.Log(_mouseDelta);
        //
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     ToggleMouseLock();
        // }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }
    }
}
