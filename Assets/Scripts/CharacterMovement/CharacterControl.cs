using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Drives the character movement state machine.
/// Attach this component to your player GameObject.
/// </summary>
public class CharacterControl : MonoBehaviour
{
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private Transform debugRot;
    [SerializeField] private Transform inputRotTransform;
    
    [Header("Gravity Settings")]
    public Vector3 _gravityDirection = Vector3.down;
    public float _gravity = 9.5f;
    public float _terminalSpeed = 50f;
    public float _jumpForce = 5f;

    public Vector3 gravityAxisFilter;
    private Vector3 igravityAxisFilter;

    public Vector3 prevVelocity;
    public Vector3 velocity;
    
    [Space]
    [Header("Movement Settings")]
    public float moveAcceleration = 5f;
    public float moveDeceleration = 5f;
    public float moveSpeed = 5f;

    [SerializeField] private GameObject killScreen;
    [SerializeField] private Timer timer;
    
    // ── Shared state the machine exposes to all states ──────────────────────
    // Add references your states will need here, e.g.:
    public Rigidbody _rb { get; private set; }
    public CapsuleCollider _collider { get; private set; }
    public Animator Anim { get; private set; }

    // ── Internal ─────────────────────────────────────────────────────────────
    private ICharacterState _currentState;

    // Pre-allocated state instances (avoids per-transition allocations)
    public readonly GroundedState Grounded  = new GroundedState();
    public readonly AirborneState Airborne  = new AirborneState();

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    private void Awake()
    {
        // Cache any component references here, e.g.:
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Enter the initial state
        TransitionTo(Airborne);
    }

    private void Update()
    {
        _currentState?.Update(this);
        _cameraFollow.position = transform.position; 
    }

    public void Jump()
    {
        velocity += -(_gravityDirection) * _jumpForce;
        TransitionTo(Airborne);
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        killScreen.gameObject.SetActive(true);
        timer.StopTimer();
        InputManager.Instance.ToggleMouseLock();
    }

    Coroutine gravityChangeCoroutine;
    
    public void ChangeGravity(Vector3 gravityDirection, Transform newPosition)
    {
        if (gravityChangeCoroutine != null) return;

        gravityChangeCoroutine = StartCoroutine(ChangeGravityRoutine(gravityDirection, newPosition));
    }

    private IEnumerator ChangeGravityRoutine(Vector3 gravityDirection, Transform newPosition)
    {
        _collider.enabled = false;
        yield return Animations.LerpTransform(this.transform, newPosition, .2f);
        yield return Animations.RotateTransform(_cameraFollow, newPosition.rotation, .2f);
        _gravityDirection = gravityDirection;
        TransitionTo(Airborne);
        gravityChangeCoroutine = null;

        yield return new WaitForSeconds(.2f);
        _collider.enabled = true;
    }

    public void CalculateGravityAxis()
    {
        igravityAxisFilter = new Vector3(Mathf.Abs(_gravityDirection.x),
            Mathf.Abs(_gravityDirection.y),
            Mathf.Abs(_gravityDirection.z));
        
        //Debug.Log($"igravityAxisFilter: {igravityAxisFilter}");
        
        gravityAxisFilter.x = igravityAxisFilter.x < 1 ? 1 : 0;
        gravityAxisFilter.y = igravityAxisFilter.y < 1 ? 1 : 0;
        gravityAxisFilter.z = igravityAxisFilter.z < 1 ? 1 : 0;
        
        
        //Debug.Log($"gravityAxisFilter: {gravityAxisFilter}");
    }

    public Quaternion GetMoveDirection()
    {
        
        // Get vector showing which way camera is looking in 2D
        Vector3 cameraFaceForward = Camera.main.transform.forward;
        
        //Debug.Log($"Camera Face: {cameraFaceForward}");
        //Debug.Log($"gravityAxisFilter: {gravityAxisFilter}");
        
        cameraFaceForward = Vector3.Scale(cameraFaceForward, gravityAxisFilter).normalized;
        
        
        // Convert forwards into Quaternions and extract the angles
        Quaternion inputRotation = Quaternion.LookRotation(InputManager.Instance.InputDir,
            Vector3.up);

        Quaternion cameraRotation = Quaternion.LookRotation(cameraFaceForward, -_gravityDirection);

        
        float inputAngle = inputRotation.eulerAngles.y;
        float cameraAngle = 0;
        Quaternion finalRotation = transform.rotation;
        
        inputRotTransform.localRotation = Quaternion.Euler(0,
            inputAngle, 
            0);
        
        inputRotation = inputRotTransform.localRotation;
        debugRot.rotation = inputRotation;

        finalRotation = cameraRotation * inputRotation;
        
        //Debug.Log($"input angle = {inputAngle}");

        // debugRot.rotation = finalRotation;
        
        return finalRotation;
    }
    
    public void AirMovementLogic()
    {
        Vector3 lateralMovement = Vector3.Scale(velocity, gravityAxisFilter);
        
        if (InputManager.Instance.InputDir == Vector3.zero)
        {
            velocity += -lateralMovement * (moveDeceleration * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;
            return;
        }

        // Make Character face said direction
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            GetMoveDirection(),
            Time.deltaTime * 10f);
        

        // Increase Velocity
        if (lateralMovement.magnitude < moveSpeed)
        {
            velocity += transform.forward * (moveAcceleration * Time.deltaTime);
        }
        else
        {
            Vector3 forward = Vector3.Scale(transform.forward, gravityAxisFilter).normalized;

            velocity = Vector3.Scale(velocity, igravityAxisFilter);
            
            velocity += forward * moveSpeed;
        }
        
        transform.position += velocity * Time.deltaTime;
    }

    public void GroundMovementLogic()
    {
        if (velocity.magnitude > 0 && prevVelocity.magnitude == 0)
        {
            Anim.CrossFade("Running", .1f);
        }
        else if (velocity.magnitude == 0 && prevVelocity.magnitude > 0)
        {
            Anim.CrossFade("Idle", .1f);
        }
        
        prevVelocity = velocity;
        
        if (InputManager.Instance.InputDir == Vector3.zero)
        {
            if (velocity.magnitude > 0.1f)
            {
                velocity += -velocity.normalized * (moveDeceleration * Time.deltaTime);
                transform.position += velocity * Time.deltaTime;
            }
            else
                velocity = Vector3.zero;
            
            return;
        }
        
        // Make Character face said direction
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            GetMoveDirection(),
            Time.deltaTime * 10f);

        // Increase Velocity
        if (velocity.magnitude < moveSpeed)
        {
            velocity += transform.forward * (moveAcceleration * Time.deltaTime);
        }
        else
        {
            velocity = transform.forward * moveSpeed;
        }
        
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        _currentState?.OnTriggerEnter(collision, this);
    }

    private void OnCollisionExit(Collision other)
    {
        TransitionTo(Airborne);
    }

    // ── Public API ────────────────────────────────────────────────────────────
    /// <summary>
    /// Transitions the machine into <paramref name="nextState"/>,
    /// calling Setup on the incoming state.
    /// </summary>
    public void TransitionTo(ICharacterState nextState)
    {
        if (_currentState == nextState) return;
        _currentState = nextState;
        _currentState.Setup(this);
    }

    /// <summary>Returns the currently active state.</summary>
    public ICharacterState CurrentState => _currentState;
}
