/// <summary>
/// Base interface for all character movement states.
/// </summary>
public interface ICharacterState
{
    void Setup(CharacterStateMachine stateMachine);
    void Update(CharacterStateMachine stateMachine);
}
