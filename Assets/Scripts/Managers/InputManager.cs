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
    
    public Action OnArrowStart;
    public Action OnArrowUpdate;
    public Action OnArrowEnd;
    
    #endregion 
    
    #region Properties

    public Vector2 MouseDelta => _mouseDelta;
    public Vector3 InputDir
    {
        get
        {
            _inputDir.Normalize();
            return new Vector3(_inputDir.x, 0, _inputDir.y);
        }
    }
    public Vector2 ArrowDir => _arrowDir;

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
    private Vector2 _arrowDir;
    private Vector2 _prevArrowDir;

    private void Update()
    {
        _inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _arrowDir = new Vector2(Input.GetAxisRaw("HArrow"), Input.GetAxisRaw("VArrow"));

        if (_arrowDir != Vector2.zero && _prevArrowDir == Vector2.zero)
        {
            OnArrowStart?.Invoke();
        }
        else if (_arrowDir == Vector2.zero && _prevArrowDir != Vector2.zero)
        {
            OnArrowEnd?.Invoke();
        }
        else if(_arrowDir != _prevArrowDir)
        {
            OnArrowUpdate?.Invoke();
        }
        
        _prevArrowDir = _arrowDir;
        //Debug.Log(_arrowDir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }
    }
}
