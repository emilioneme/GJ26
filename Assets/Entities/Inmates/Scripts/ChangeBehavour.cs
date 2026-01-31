using UnityEngine;
using UnityEngine.Events;

public class ChangeBehavour : MonoBehaviour
{
    [SerializeField] float changeInterval = 15f;
    [SerializeField] float randomIntervalRange = 5f;

    public UnityEvent ChangeBehavuour; 

    float lastChangeTime;
    private void Start()
    {
        changeInterval
                = Random.Range(changeInterval - randomIntervalRange, changeInterval + randomIntervalRange);
    }
    public void Update()
    {
        if (Time.time - lastChangeTime >= changeInterval)
        {
            ChangeBehavuour.Invoke();
            lastChangeTime = Time.time;
            changeInterval 
                = Random.Range(changeInterval - randomIntervalRange, changeInterval + randomIntervalRange);
        }
    }
}
