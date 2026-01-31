using UnityEngine;

public class BlendingHandler : MonoBehaviour
{
    public float currentBlendingEfficacy = 0;
    public GameObject standingTarget = null;

    public void UpdateBlendEfficacy(Vector2 avgDirection) 
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
            currentBlendingEfficacy = dot;
        } 
        else //not in standing group
        {
            if(avgDirection.sqrMagnitude < 0.0001f)
            {
                currentBlendingEfficacy = -1;
                return;
            }

            Vector2 currentDirection = new Vector2(transform.forward.x, transform.forward.z);
            float dot = Vector2.Dot(currentDirection.normalized, avgDirection.normalized);
            currentBlendingEfficacy = dot;
        } 
    }
}
