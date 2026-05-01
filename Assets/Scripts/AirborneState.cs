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

        if (control.velocity.magnitude > control._terminalSpeed)
        {
            control.velocity = control.velocity.normalized * control._terminalSpeed;
        }

        control.transform.position += control.velocity * Time.deltaTime;
        //Debug.Log("Velocity: " + control.velocity);
    }

    void ICharacterState.OnCollisionEnter(Collider collision, CharacterControl control)
    {
        //Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
        control.TransitionTo(control.Grounded);
    }
}
