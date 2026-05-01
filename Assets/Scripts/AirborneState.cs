/// <summary>
/// State representing the character while airborne or falling.
/// </summary>
public class AirborneState : ICharacterState
{
    public void Setup(CharacterStateMachine stateMachine)
    {
        // Initialise airborne behaviour here (e.g. enable fall animations, store launch velocity)
    }

    public void Update(CharacterStateMachine stateMachine)
    {
        // Handle airborne-specific logic each frame here (e.g. air control, gravity scaling)
    }
}
