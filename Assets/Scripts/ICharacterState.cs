using UnityEngine;

/// <summary>
/// Base interface for all character movement states.
/// </summary>
public interface ICharacterState
{
    
    
    void Setup(CharacterControl control);
    void Update(CharacterControl control);
    void OnCollisionEnter(Collider collision, CharacterControl control);
}
