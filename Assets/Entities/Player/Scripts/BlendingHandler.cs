using Unity.VisualScripting;
using UnityEngine;

public class BlendingHandler : MonoBehaviour
{
    public float currentBlendingEfficacy = 0;
    public float previousBlendingEfficacy = 0;
    public float blendingSpeed = 1;
    public float minSpeed = 1;
    public GameObject standingTarget = null;

    [SerializeField] MovingController controller;

    bool beingWatched = false;

    private void Update()
    {
        SmoothenBlending();
    }

    public void UpdateBlendEfficacy(Vector2 avgDirection, bool walking) 
    {
        //in standing group
        if(standingTarget != null)
        {
            Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;

            Vector2 toTarget2D = new Vector2(
                standingTarget.transform.position.x - transform.position.x,
                standingTarget.transform.position.z - transform.position.z
            ).normalized;

            float dot = Vector2.Dot(forward2D, toTarget2D);
            previousBlendingEfficacy = dot;
        } 
        else //not in standing group
        {
            if(avgDirection.sqrMagnitude < 0.0001f)
            {
                previousBlendingEfficacy = -1;
                return;
            }

            Vector2 currentDirection = new Vector2(transform.forward.x, transform.forward.z);
            float dot = Vector2.Dot(currentDirection.normalized, avgDirection.normalized);
            previousBlendingEfficacy = dot;

            if ((controller.moveInput.magnitude > 0.4f) != walking)
                previousBlendingEfficacy = Mathf.Clamp(previousBlendingEfficacy - .5f, -1, 1);

            if (controller.sprintCoroutine != null)
                previousBlendingEfficacy = Mathf.Clamp(previousBlendingEfficacy - .5f, -1, 1);

        }
        SmoothenBlending();
    }
    public void SmoothenBlending()
    {
        float difference = Mathf.Abs(previousBlendingEfficacy - currentBlendingEfficacy);

        float speed = Mathf.Max(minSpeed, difference * blendingSpeed);

        currentBlendingEfficacy = Mathf.MoveTowards(
            currentBlendingEfficacy,
            previousBlendingEfficacy,
            speed * Time.deltaTime
        );
    }


}
