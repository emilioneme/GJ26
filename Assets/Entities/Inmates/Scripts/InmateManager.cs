using UnityEngine;

public class InmateManager : MonoBehaviour
{
    public MovingController movingController;
    public int inmateBatchID = 0;

    enum InmateState
    {
        Idle,
        Moving,
        Interacting
    }

    private void Awake()
    {
        movingController = GetComponent<MovingController>();
    }

    private void Start()
    {
        inmateBatchID = Random.Range(0, GameManager.Instance.numberOfBacthes);
    }

}
