using UnityEngine;

public class BlendingHandler : MonoBehaviour
{
    public float currentBlendingEfficacy = 0;

    public void UpdateBlendEfficacy(Vector2 avgDirection) 
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
