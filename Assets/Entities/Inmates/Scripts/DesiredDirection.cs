using UnityEngine;
using UnityEngine.Events;

public class DesiredDirection : MonoBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform target;

    public UnityEvent<Vector2> onDesiredDirectionUpdated;

    private void Start()
    {
        if (targets.Length > 0)
        {
            int randomIndex = Random.Range(0, targets.Length);
            target = targets[randomIndex];
        }
    }

    public void Update() 
    {
        Vector2 self = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos = new Vector2(target.position.x, target.position.z);
        Vector2 desiredDirection = -(self - targetPos).normalized;
        onDesiredDirectionUpdated.Invoke(desiredDirection);
    }

}