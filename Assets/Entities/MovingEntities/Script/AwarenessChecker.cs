using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class AwarenessSphere : MonoBehaviour
{
    public UnityEvent<Vector2> averageDirection;

    [SerializeField] public float maxInmateDistance = 10f;
    [SerializeField] bool idleInfluence = false;

    [SerializeField] Transform awarnessCenter;
    [SerializeField] float awarenessSize;

    private Collider[] InmateColliders = new Collider[30];

    [SerializeField] bool drawGizmos = false;

    void FixedUpdate()
    {
        //int batch = Random.Range(0, 5); // assuming 5 batches
        int count = Physics.OverlapSphereNonAlloc(awarnessCenter.position, awarenessSize, InmateColliders);
        var list = InmateColliders
            .Take(count)
            .Select(collider => collider.GetComponent<InmateManager>())
            .Where(manager => manager != this)
            .Where(manager => manager != null)
            //.Where(manager => manager.inmateBatchID != batch)
            .Where(manager => manager.movingController.moveInput.magnitude < 0.1f && !idleInfluence)
            ;

        Vector2 direction =
            list
            .Select
            (manager =>{
                Vector2 direction = new Vector2(manager.transform.forward.x, manager.transform.forward.z);
                float distance = Vector3.Distance(manager.transform.position, transform.position);
                distance = Mathf.Clamp01(distance / maxInmateDistance);
                float distanceFactor = 1 - distance;

                return direction * distanceFactor;
            })
            .Aggregate(Vector2.zero, (acc, dir) => acc + dir)
            ;
        direction /= list.Count();
        averageDirection.Invoke(direction);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(drawGizmos)
            Gizmos.DrawWireSphere(awarnessCenter.position, awarenessSize);
    }
}

