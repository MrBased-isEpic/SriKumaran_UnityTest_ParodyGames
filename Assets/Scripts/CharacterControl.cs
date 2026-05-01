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
        velocity = -(_gravityDirection) * _jumpForce;
        TransitionTo(Airborne);
    }

    private void OnTriggerEnter(Collider collision)
    {
        _currentState?.OnCollisionEnter(collision, this);
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
