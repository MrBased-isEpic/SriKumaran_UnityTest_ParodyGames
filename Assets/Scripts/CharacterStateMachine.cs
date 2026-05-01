using UnityEngine;

/// <summary>
/// Drives the character movement state machine.
/// Attach this component to your player GameObject.
/// </summary>
public class CharacterStateMachine : MonoBehaviour
{
    // ── Shared state the machine exposes to all states ──────────────────────
    // Add references your states will need here, e.g.:
    //   public Rigidbody2D Rb { get; private set; }
    //   public Animator    Anim { get; private set; }

    // ── Internal ─────────────────────────────────────────────────────────────
    private ICharacterState _currentState;

    // Pre-allocated state instances (avoids per-transition allocations)
    public readonly GroundedState  Grounded  = new GroundedState();
    public readonly AirborneState  Airborne  = new AirborneState();

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    private void Awake()
    {
        // Cache any component references here, e.g.:
        //   Rb   = GetComponent<Rigidbody2D>();
        //   Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Enter the initial state
        TransitionTo(Grounded);
    }

    private void Update()
    {
        _currentState?.Update(this);
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
