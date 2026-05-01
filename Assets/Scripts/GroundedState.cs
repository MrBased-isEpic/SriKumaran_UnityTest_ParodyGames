using UnityEngine;

/// <summary>
/// State representing the character while on the ground.
/// </summary>
public class GroundedState : ICharacterState
{
    public void Setup(CharacterControl control)
    {
        control.velocity = Vector3.zero;
        control._collider.isTrigger = false;
        control._rb.isKinematic = false;
        
        InputManager.Instance.OnJumpPressed += control.Jump;
    }

    public void Update(CharacterControl control)
    {
        
    }
    
    void ICharacterState.OnCollisionEnter(Collider collision, CharacterControl control)
    {
        //Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
    }
}
