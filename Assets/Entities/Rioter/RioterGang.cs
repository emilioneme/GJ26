using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RioterGang : MonoBehaviour
{
    [SerializeField] List<DesiredDirection> Followers = new List<DesiredDirection>();
    [SerializeField] Transform target;

    private void Awake()
    {
        Followers = new List<DesiredDirection>(
            FindObjectsByType<DesiredDirection>(FindObjectsSortMode.None)
        );
    }

    public void StartRiot() 
    {
        foreach (var f in Followers)
        {
            f.SetNewTarget(target);
        }
    }
}
