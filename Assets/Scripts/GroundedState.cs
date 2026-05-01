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
        if (InputManager.Instance.InputDir == Vector3.zero)
        {
            return;
        }
        
        // Get vector showing which way camera is looking in 2D
        Vector3 cameraFaceForward = Camera.main.transform.forward;
        cameraFaceForward.y = 0;
        cameraFaceForward.Normalize();
        
        // Convert forwards into Quaternions and extract the angles
        Quaternion inputRotation = Quaternion.LookRotation(InputManager.Instance.InputDir,
            -control._gravityDirection);

        Quaternion cameraRotation = Quaternion.LookRotation(cameraFaceForward,
            -control._gravityDirection);
        
        float inputAngle = inputRotation.eulerAngles.y;
        float cameraAngle = cameraRotation.eulerAngles.y;
        
        // Calculate the final rotation by adding the input to camera angle
        Quaternion finalRotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 
            AddAngle(cameraAngle, inputAngle),
            cameraRotation.eulerAngles.z);
        
        // Make Character face said direction
        control.transform.rotation = Quaternion.Lerp(control.transform.rotation, 
            finalRotation,
            Time.deltaTime * 10f);

        // Increase Velocity
        if (control.velocity.magnitude < control.moveSpeed)
        {
            control.velocity += control.transform.forward * (control.moveAcceleration * Time.deltaTime);
        }
        else
        {
            control.velocity = control.transform.forward * control.moveSpeed;
        }
        
        control.transform.position += control.velocity * Time.deltaTime;
    }

    float AddAngle(float a, float b)
    {
        float added = a + b;
        if (added > 360)
        {
            return added - 360;
        }
        else
        {
            return added;
        }
    }
    
    void ICharacterState.OnTriggerEnter(Collider collision, CharacterControl control)
    {
        //Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
    }
}
