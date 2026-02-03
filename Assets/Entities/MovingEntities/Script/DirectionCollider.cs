using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DirectionCollider : MonoBehaviour
{
    public GameObject self;
    public UnityEvent<Vector2, bool> averageDirection; //bool of walking or notd

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
            averageDirection.Invoke(Vector2.zero, true);
            return;
        }

        Dictionary<bool, int> numberOfWalkers = new Dictionary<bool, int>
        {
            { true, 0 },
            { false, 0 }
        };
        Vector2 cumulativeDirection = Vector2.zero;
        foreach (GameObject boid in Inmates)
        {
            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction;

            if (boid.TryGetComponent(out MovingController mc))
            {
                if (mc.moveInput.magnitude > .1f)
                    numberOfWalkers[true] += 1;
                else
                    numberOfWalkers[false] += 1;
            }
        }

        bool walkingAverage = true;
        if (numberOfWalkers[true] > numberOfWalkers[false])
            walkingAverage = true;
        Vector2 avgDirection = cumulativeDirection / Inmates.Count;
        averageDirection.Invoke(avgDirection.normalized, walkingAverage);
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
