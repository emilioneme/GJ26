using UnityEngine;

public class UserData : MonoBehaviour
{
    UserData Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public float volume = 1.0f;

    [SerializeField] public float sensitiviy = 0;
}
