using UnityEngine;
using UnityEngine.Events;

public class InmateRotationInput : MonoBehaviour
{
    [SerializeField] UnityEvent<float> onSteer; // -1..1

    [SerializeField] float deadZoneDegrees = 2f;

    [SerializeField] float fullSteerDegrees = 45f;
    [SerializeField] float steerSmoothTime = 0.25f;  // bigger = slower, smoother
    [SerializeField] float maxSteerSpeed = 10f;      // limits how fast it can change

    [SerializeField] bool randomAtStart = false;

    [SerializeField] float steerVelocity = 10f;

    [SerializeField] bool debug = false;

    float randomSteer = 0f;
    float currentSteer = 0f;

    void Start()
    {
        PickRandomSteer();
    }

    public void UpdateDesiredRotation(Vector2 direction) // avg direction
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            currentSteer = Mathf.SmoothDamp(
                currentSteer,
                0f,
                ref steerVelocity,
                steerSmoothTime,
                maxSteerSpeed,
                Time.deltaTime
            );

            onSteer.Invoke(currentSteer);
            return;
        }

        Vector2 currentForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 desired = direction.normalized;

        if (debug) 
        {
            Debug.DrawRay(transform.position, new Vector3(currentForward.x, 0f, currentForward.y), Color.red);
            Debug.DrawRay(transform.position, new Vector3(desired.x, 0f, desired.y), Color.green);
        }
            

        float signedAngle = Vector2.SignedAngle(currentForward, desired);
        signedAngle = Mathf.Clamp(signedAngle, -fullSteerDegrees, fullSteerDegrees);

        float desiredSteer;

        if (Mathf.Abs(signedAngle) < deadZoneDegrees)
        {
            desiredSteer = randomSteer;
        }
        else
        {
            desiredSteer = Mathf.Clamp(-signedAngle / fullSteerDegrees, -1f, 1f);
        }


        currentSteer = Mathf.SmoothDamp(
            currentSteer,
            desiredSteer,
            ref steerVelocity,
            steerSmoothTime,
            maxSteerSpeed,
            Time.deltaTime
        );

        onSteer.Invoke(desiredSteer);
        //onSteer.Invoke(currentSteer);
        //Debug.Log(currentSteer);
    }
    public void PickRandomSteer()
    {
        randomSteer = Random.Range(-1f, 1f);
    }
}
