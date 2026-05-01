using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Drives the character movement state machine.
/// Attach this component to your player GameObject.
/// </summary>
public class CharacterControl : MonoBehaviour
{
    
    [Header("Gravity Settings")]
    public Vector3 _gravityDirection = Vector3.down;
    public float _gravity = 9.5f;
    public float _terminalSpeed = 50f;
    public float _jumpForce = 5f;
    
    public Vector3 velocity;
    
    [Space]
    [Header("Movement Settings")]
    public float moveAcceleration = 5f;
    public float moveDeceleration = 5f;
    public float moveSpeed = 5f;
    
    
    
    // ── Shared state the machine exposes to all states ──────────────────────
    // Add references your states will need here, e.g.:
    public Rigidbody _rb { get; private set; }
    public CapsuleCollider _collider { get; private set; }
    //   public Animator    Anim { get; private set; }

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
        //   Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Enter the initial state
        TransitionTo(Airborne);
    }

    private void Update()
    {
        _currentState?.Update(this);
    }

    public void Jump()
    {
        Debug.Log("Jumping");
        velocity += -(_gravityDirection) * _jumpForce;
        TransitionTo(Airborne);
    }

    public Quaternion GetMoveDirection()
    {
        // Get vector showing which way camera is looking in 2D
        Vector3 cameraFaceForward = Camera.main.transform.forward;
        cameraFaceForward.y = 0;
        cameraFaceForward.Normalize();
        
        // Convert forwards into Quaternions and extract the angles
        Quaternion inputRotation = Quaternion.LookRotation(InputManager.Instance.InputDir,
            -_gravityDirection);

        Quaternion cameraRotation = Quaternion.LookRotation(cameraFaceForward,
            -_gravityDirection);
        
        float inputAngle = inputRotation.eulerAngles.y;
        float cameraAngle = cameraRotation.eulerAngles.y;
        
        // Calculate the final rotation by adding the input to camera angle
        Quaternion finalRotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 
            AddAngle(cameraAngle, inputAngle),
            cameraRotation.eulerAngles.z);
        
        return finalRotation;
    }
    
    public void AirMovementLogic()
    {
        Vector3 lateralMovement = velocity;
        lateralMovement.y = 0;
        
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
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            velocity.x = 0;
            velocity.z = 0;
            
            velocity += forward * moveSpeed;
        }
        
        transform.position += velocity * Time.deltaTime;
    }

    public void GroundMovementLogic()
    {
        if (InputManager.Instance.InputDir == Vector3.zero)
        {
            velocity += -velocity.normalized * (moveDeceleration * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;

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
    
    float AddAngle(float a, float b)
    {
        float added = a + b;
        if (added > 360)
        {
            return added - 360;
        }
        else
        {
            return added;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        _currentState?.OnTriggerEnter(collision, this);
    }

    // ── Public API ────────────────────────────────────────────────────────────
    /// <summary>
    /// Transitions the machine into <paramref name="nextState"/>,
    /// calling Setup on the incoming state.
    /// </summary>
    public void TransitionTo(ICharacterState nextState)
    {
        _currentState = nextState;
        _currentState.Setup(this);
    }

    /// <summary>Returns the currently active state.</summary>
    public ICharacterState CurrentState => _currentState;
}
