using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DirectionCollider : MonoBehaviour
{
    public GameObject self;
    public UnityEvent<Vector2> averageDirection;

    public List<GameObject> Inmates = new List<GameObject>();

    private void Awake()
    {
        if (self == null)
            self = transform.root.gameObject; // root of this child
    }

    void Update()
    {
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
        foreach (GameObject boid in Inmates)
        {
            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction;
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
