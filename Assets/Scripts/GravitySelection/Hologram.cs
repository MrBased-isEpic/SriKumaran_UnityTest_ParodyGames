using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram : MonoBehaviour
{
    [SerializeField] private Transform localRotator;
    [SerializeField] private Transform hologramTransform;
    
    [SerializeField] private CharacterControl _control;

    private Vector3[] _axes = {
        new (0,0,-1),
        new (0,0,1),
        new (1,0,0),
        new (-1,0,0),
        new (0,1,0),
        new (0,-1,0),
    };

    void Start()
    {
        localRotator.gameObject.SetActive(false);

        InputManager.Instance.OnArrowStart += OnArrowStarted;
        InputManager.Instance.OnArrowEnd += OnArrowEnded;
        InputManager.Instance.OnArrowUpdate += OnArrowUpdate;
        
        InputManager.Instance.OnGravityPressed += OnGravityPressed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = GetClosestCameraForwardAxis();
        transform.rotation = Quaternion.LookRotation(forward, -_control._gravityDirection);

        if (!localRotator.gameObject.activeSelf) return;
        
        localRotator.localRotation = Quaternion.Lerp(localRotator.localRotation,
            _targetRotation, Time.deltaTime * 10);
    }
    
    Quaternion _targetRotation = Quaternion.identity;

    void OnArrowStarted()
    {
        localRotator.gameObject.SetActive(true);
        OnArrowUpdate();
    }

    void OnArrowEnded()
    {
        localRotator.gameObject.SetActive(false);
    }

    void OnArrowUpdate()
    {
        Vector2 input = InputManager.Instance.ArrowDir;

        if (input.x != 0)
        {
            _targetRotation = Quaternion.Euler(0,0, 90 * input.x);
        }
        else
        {
            _targetRotation = Quaternion.Euler(-90 * input.y,0,0);
        }
    }
    
    private void OnGravityPressed()
    {
        localRotator.localRotation = _targetRotation;
        Vector3 gravityDirection = (hologramTransform.position - transform.position).normalized;
        _control.ChangeGravity(gravityDirection, hologramTransform);
    }

    Vector3 GetClosestCameraForwardAxis()
    {
        // Get vector showing which way camera is looking in 2D
        Vector3 cameraFaceForward = Camera.main.transform.forward;

        float highestDot = 0;
        int closestAxisIndex = -1;
        for (var index = 0; index < _axes.Length; index++)
        {
            float dot = Vector3.Dot(cameraFaceForward, _axes[index]);
            if (dot > highestDot)
            {
                highestDot = dot;
                closestAxisIndex = index;
            }
        }
        
        return _axes[closestAxisIndex];
    }
}
