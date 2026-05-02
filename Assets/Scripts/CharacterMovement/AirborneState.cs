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
        
        control.Anim.CrossFade("Falling Idle", .1f);
    }

    public void Update(CharacterControl control)
    {
        control.CalculateGravityAxis();
        
        control.velocity += control._gravityDirection * (control._gravity * Time.deltaTime);

        if (Mathf.Abs(control.velocity.y) > control._terminalSpeed)
        {
            control.velocity = control.velocity.normalized * control._terminalSpeed;
        }
        
        control.AirMovementLogic();
    }

    void ICharacterState.OnTriggerEnter(Collider collision, CharacterControl control)
    {
        if (collision.CompareTag("Respawn"))
        {
            control.gameObject.SetActive(false);
            return;
        }

        if (!collision.CompareTag("Ground")) return;
        
        // Making sure it's a floor and not a "Wall" before stopping your fall.
        float dot = Vector3.Dot(collision.transform.up, control._gravityDirection);
        
        if (Mathf.Approximately(dot, 1) || Mathf.Approximately(dot, -1))
        {
            control.TransitionTo(control.Grounded);
        }
        
    }
}
