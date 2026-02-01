using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    static public UserData Instance;
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

    [SerializeField] AudioListener audioListener;

    [SerializeField] public float volume = 1.0f;

    [SerializeField] public float sensitiviy = 0;

    void Start()
    {
        SetVolume(volume);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void SetVolume(float value) 
    {
        AudioListener.volume = 1f;
        volume = value;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SetVolume(volume);
    }
}
