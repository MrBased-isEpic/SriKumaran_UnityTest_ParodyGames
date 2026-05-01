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
    public Vector3 InputDir
    {
        get
        {
            _inputDir.Normalize();
            return new Vector3(_inputDir.normalized.x, 0,_inputDir.normalized.y);
        }
    }

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
    private Vector2 _inputDir;

    private void Update()
    {
        _inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Debug.Log(_inputDir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }
    }
}
