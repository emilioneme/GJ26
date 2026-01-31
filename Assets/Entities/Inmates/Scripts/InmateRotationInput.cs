using UnityEngine;
using UnityEngine.Events;

public class InmateRotationInput : MonoBehaviour
{
    [SerializeField] UnityEvent<float> onSteer; // -1..1
    [SerializeField] float deadZoneDegrees = 2f;
    [SerializeField] float fullSteerDegrees = 45f;

    public void UpdateDesiredRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            onSteer.Invoke(0f);
            return;
        }

        Vector2 currentForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 desired = direction.normalized;

        float signedAngle = Vector2.SignedAngle(currentForward, desired);

        // dead zone
        if (Mathf.Abs(signedAngle) < deadZoneDegrees)
        {
            onSteer.Invoke(0f);
            return;
        }

        // map angle -> -1..1
        float steer = Mathf.Clamp(signedAngle / fullSteerDegrees, -1f, 1f);

        onSteer.Invoke(steer);
    }
}
