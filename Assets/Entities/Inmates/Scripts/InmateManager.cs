using UnityEngine;

public class InmateManager : MonoBehaviour
{
    public MovingController movingController;
    public int inmateBatchID;
    enum InmateState
    {
        Idle,
        Moving,
        Interacting
    }

    private void Awake()
    {
        inmateBatchID = Random.Range(0, 5);
        movingController = GetComponent<MovingController>();
    }

}
