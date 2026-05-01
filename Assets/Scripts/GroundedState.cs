using System.Numerics;
using Cinemachine.Utility;
using UnityEngine;
using Input = UnityEngine.Windows.Input;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
        control.GroundMovementLogic();
    }
    
    
    void ICharacterState.OnTriggerEnter(Collider collision, CharacterControl control)
    {
        //Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
    }
}
