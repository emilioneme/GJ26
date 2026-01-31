using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class DirectionCollider : MonoBehaviour
{
    public GameObject self;
    public UnityEvent<Vector2> averageDirection;

    [SerializeField] public float maxInmateDistance = 10f;
    [SerializeField] public float distanceMultiplier = 1;
    public List<GameObject> Inmates = new List<GameObject>();

    //[SerializeField] float idleInfleunce = 0
    [SerializeField] bool idleInfluence = false;

    private void Awake()
    {
        if (self == null)
            self = transform.root.gameObject; // root of this child
    }

    private Collider[] InmateColliders;

    void Update()
    {
        /*
        int count = Physics.OverlapSphereNonAlloc(transform.position, 10, InmateColliders);
        var list = InmateColliders
            .Take(count)
            // remove yuourself from the list
            .Select(collider => collider.GetComponent<MovingController>())
            .Where(controller => controller !=)
            .Where(controller => controller != null)
            .Where(controller => controller.moveInput.magnitude < 0.1f && !idleInfluence)
            ;

        Vector2 direction = list
            //.Select(controller => new Vector2(controller.transform.forward.x, controller.transform.forward.z))
            //.Select(direction => direction * distanceFactor)
            
            .Select
            (   controller =>
                {
                    Vector2 direction = new Vector2(controller.transform.forward.x, controller.transform.forward.z);
                    float distance = Vector3.Distance(controller.transform.position, self.transform.position);
                    distance = Mathf.Clamp01(distance / maxInmateDistance);
                    float distanceFactor = 1 - distance;

                    return direction * distanceFactor;
                }
            )
            .Aggregate(Vector2.zero, (acc, dir) => acc + dir)
            ;
        direction /= list.Count();
        */

        // Clean nulls (in case inmates despawn/disable)
        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            if (Inmates[i] == null)
                Inmates.RemoveAt(i);
        }

        if (Inmates.Count == 0)
        {
            averageDirection.Invoke(Vector2.zero);
            return;
        }

        Vector2 cumulativeDirection = Vector2.zero;
        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            GameObject boid = Inmates[i];

            float distance = Vector3.Distance(boid.transform.position, self.transform.position);
            distance = Mathf.Clamp01(distance / maxInmateDistance);
            float distanceFactor = 1 - distance;

            //bool isIdle = false;
            if (boid.TryGetComponent(out MovingController mc))
            {
                if (mc.moveInput.magnitude < 0.1f && !idleInfluence) 
                {
                    Inmates.Remove(boid.gameObject);
                    continue;
                }
            }

            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction * distanceFactor;
        }

        Vector2 avgDirection = cumulativeDirection / Inmates.Count;


        averageDirection.Invoke(avgDirection.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inmate") && other.gameObject != self)
        {
            if (!Inmates.Contains(other.gameObject))
                Inmates.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inmate") && other.gameObject != self)
        {
            Inmates.Remove(other.gameObject);
        }
    }
}
