using Entities.Inmates;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(InmateInput))]
[RequireComponent(typeof(CharacterController))]
public class InmateCollsions : MonoBehaviour
{
    CharacterController cc;
    InmateInput input;

    private void Awake()
    {
        input = GetComponent<InmateInput>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle"))
        {
            input.forwardDirection = hit.normal - Vector3.Dot(hit.normal, Vector3.up)*Vector3.up;
            // add slight random rotation 
            input.forwardDirection = Quaternion.Euler(0,Random.value*15,0)*input.forwardDirection;
        }
    }
}

