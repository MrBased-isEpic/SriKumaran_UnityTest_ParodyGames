/// <summary>
/// State representing the character while on the ground.
/// </summary>
public class GroundedState : ICharacterState
{
    public void Setup(CharacterStateMachine stateMachine)
    {
        // Initialise grounded behaviour here (e.g. enable walk animations, reset jump count)
    }

    public void Update(CharacterStateMachine stateMachine)
    {
        // Handle grounded-specific logic each frame here (e.g. movement input, jump detection)
    }
}
