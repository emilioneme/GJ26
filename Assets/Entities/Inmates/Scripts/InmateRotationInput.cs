using UnityEngine;
using UnityEngine.Events;

public class InmateRotationInput : MonoBehaviour
{
    [SerializeField] UnityEvent<float> onSteer; // -1..1


    [SerializeField] float deadZoneDegrees = 2f;
    [SerializeField] float fullSteerDegrees = 45f;

    [SerializeField][Range(-1, 1)] float alignentMag = 1;
    [SerializeField] float steerSmoothTime = 0.25f;  // bigger = slower, smoother
    [SerializeField] float maxSteerSpeed = 10f;      // limits how fast it can change

    float currentSteer = 0f;
    float steerVelocity = 0f;

    public void UpdateDesiredRotation(Vector2 direction) //avrg direction
    {
        Vector2 dir = direction;

        float alignent = alignentMag;

        Vector2 currentForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 desired = dir.normalized;

        Debug.DrawRay(transform.position, new Vector3(currentForward.x, 0f, currentForward.y), Color.red);
        Debug.DrawRay(transform.position, new Vector3(desired.x, 0f, desired.y), Color.green);

        float signedAngle = Vector2.SignedAngle(currentForward, desired);
        //float steer = Mathf.Clamp(-signedAngle * alignent / fullSteerDegrees, -1f, 1f);
        float desiredSteer;

        if (Mathf.Abs(signedAngle) < deadZoneDegrees)
            desiredSteer = Random.Range(-1, 1); 
        else
            desiredSteer = Mathf.Clamp(-signedAngle * alignent / fullSteerDegrees, -1f, 1f);

        currentSteer = Mathf.SmoothDamp(
            currentSteer,
            desiredSteer,
            ref steerVelocity,
            steerSmoothTime,
            maxSteerSpeed,
            Time.deltaTime
        );

        onSteer.Invoke(currentSteer);
    }

    public void ChangeBehaviour() 
    {
        alignentMag = Mathf.Sign(Random.Range(-1, 1f));
    }
}
