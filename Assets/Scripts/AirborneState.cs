using UnityEngine;

/// <summary>
/// State representing the character while airborne or falling.
/// </summary>
public class AirborneState : ICharacterState
{
    public void Setup(CharacterControl control)
    {
        control._collider.isTrigger = true;
        control._rb.isKinematic = true;

        InputManager.Instance.OnJumpPressed -= control.Jump;
    }

    public void Update(CharacterControl control)
    {
        control.velocity += control._gravityDirection * (control._gravity * Time.deltaTime);

        if (Mathf.Abs(control.velocity.y) > control._terminalSpeed)
        {
            control.velocity = control.velocity.normalized * control._terminalSpeed;
        }
        
        control.AirMovementLogic();
    }

    void ICharacterState.OnTriggerEnter(Collider collision, CharacterControl control)
    {
        // Making sure it's a floor and not a "Wall" before stopping your fall.
        float dot = Vector3.Dot(collision.transform.up, control._gravityDirection);
        if (dot == -1 || dot == 1)
        {
            Debug.Log(dot);
            control.TransitionTo(control.Grounded);
            Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
        }
        
    }
}
