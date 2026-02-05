using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    [SerializeField] GameObject brokenSound;

    [SerializeField] GameObject raiseAlertSound;

    [SerializeField] GameObject riotSound;

    [SerializeField] GameObject pcikedUpValuableSound;

    public void RaiseAlertSound() 
    {
        Destroy(Instantiate(raiseAlertSound, Vector3.zero, Quaternion.identity), 5);
    }

    public void BrokenSound(Vector3 pos)
    {
        Destroy(Instantiate(brokenSound, pos, Quaternion.identity), 5);
    }

    public void ValuableSound()
    {
        Destroy(Instantiate(pcikedUpValuableSound), 5f);
    }

    public void RiotSound()
    {
       Instantiate(riotSound);
    }
}
