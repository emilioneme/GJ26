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

    [SerializeField] GameObject raiseAlertSound;

    public void RaiseAlertSound() 
    {
        Destroy(Instantiate(raiseAlertSound), 1);
    }

    
}
