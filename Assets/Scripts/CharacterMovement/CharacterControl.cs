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

    public void CalculateGravityAxis()
    {
        igravityAxisFilter = new Vector3(Mathf.Abs(_gravityDirection.x),
            Mathf.Abs(_gravityDirection.y),
            Mathf.Abs(_gravityDirection.z));
        
        gravityAxisFilter.x = igravityAxisFilter.x == 0 ? 1 : 0;
        gravityAxisFilter.y = igravityAxisFilter.y == 0 ? 1 : 0;
        gravityAxisFilter.z = igravityAxisFilter.z == 0 ? 1 : 0;
    }

    public Quaternion GetMoveDirection()
    {
        //Debug.Log($"Gravity Axis: {cameraAxisFilter}");
        
        // Get vector showing which way camera is looking in 2D
        Vector3 cameraFaceForward = Camera.main.transform.forward;
        cameraFaceForward = Vector3.Scale(cameraFaceForward, gravityAxisFilter).normalized;
        
        //Debug.Log($"Camera Face: {cameraFaceForward}");
        
        // Convert forwards into Quaternions and extract the angles
        Quaternion inputRotation = Quaternion.LookRotation(InputManager.Instance.InputDir,
            -_gravityDirection);

        Quaternion cameraRotation = Quaternion.LookRotation(cameraFaceForward,
            -_gravityDirection);
        
        //Debug.Log($"Camera Rotation: {cameraRotation.eulerAngles}");

        float inputAngle = 0;
        float cameraAngle = 0;
        Quaternion finalRotation = Quaternion.Euler(0,0,0);
        
        
        // Calculate the final rotation by adding the input to camera angle
        if (Mathf.Abs(_gravityDirection.x) == 1)
        {
            inputAngle = inputRotation.eulerAngles.x;
            cameraAngle = cameraRotation.eulerAngles.x;
            finalRotation = Quaternion.Euler(AddAngle(cameraAngle, inputAngle), 
                cameraRotation.eulerAngles.y,
                cameraRotation.eulerAngles.z);
        }
        else if (Mathf.Abs(_gravityDirection.y) == 1)
        {
            inputAngle = inputRotation.eulerAngles.y;
            cameraAngle = cameraRotation.eulerAngles.y;
            finalRotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 
                AddAngle(cameraAngle, inputAngle),
                cameraRotation.eulerAngles.z);
        }
        else
        {
            //Debug.Log("Calculating z axis");
            inputAngle = inputRotation.eulerAngles.x;
            cameraAngle = cameraRotation.eulerAngles.x;
            finalRotation = Quaternion.Euler(AddAngle(cameraAngle, inputAngle), 
                cameraRotation.eulerAngles.y,
                cameraRotation.eulerAngles.z);
            // inputAngle = inputRotation.eulerAngles.z;
            // cameraAngle = cameraRotation.eulerAngles.z;
            // finalRotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 
            //     cameraRotation.eulerAngles.y,
            //     AddAngle(cameraAngle, inputAngle));
        }

        debugRot.rotation = finalRotation;
        //Debug.Log($"Final Rotation : {finalRotation.eulerAngles}");
        
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
