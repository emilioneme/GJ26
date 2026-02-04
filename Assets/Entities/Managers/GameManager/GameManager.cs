using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public AudioSource Scaryaudio;
    public static GameManager Instance { get; set; }

    public GameObject player;

    public GameObject spotlight;

    public string level;

    [SerializeField] SpotlightSpin sp;

    //level design
    public GameObject BridgeBlocker;
    public Transform EndingTransform;

    [SerializeField] public AudioSource highAlertAudio;


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

        level = SceneManager.GetActiveScene().name;
        Debug.Log(level);
        Debug.Log(level.Equals("Level2"));

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        RaiseAlert(0);
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sp = spotlight.GetComponent<SpotlightSpin>();
    }

    public void HighAlert()
    {
        if(UserData.Instance.alertBarAmount < 75)
        {
            UserData.Instance.alertBarAmount = 75;
        }
    }

    public void MaxDifficulty() 
    {
        player.GetComponent<PlaverManager>().LostGame();
    }

    public void RaiseAlert(float alertAmount)
    {
        UserData.Instance.alertBarAmount = Math.Clamp(UserData.Instance.alertBarAmount + alertAmount, 0, 100);

        if(UserData.Instance.alertBarAmount < 33 && UserData.Instance.alertLevel > 0 && !level.Equals("Level2"))
        {
            sp.playerTarget = false;
            //spc.enabled = true;
            UserData.Instance.alertLevel = 0;
            Debug.Log("alert level: " + UserData.Instance.alertLevel);
        }


        if ((UserData.Instance.alertBarAmount > 33 && UserData.Instance.alertBarAmount < 66 && UserData.Instance.alertLevel != 1) || level.Equals("Level2") && UserData.Instance.alertLevel != 1 && UserData.Instance.alertBarAmount < 66)
        {
            sp.playerTarget = false;
            //spc.enabled = true;
            UserData.Instance.alertLevel = 1;
            Debug.Log("alert level: " + UserData.Instance.alertLevel);
        }

        if (UserData.Instance.alertBarAmount > 66 && UserData.Instance.alertLevel != 2)
        {
            sp.playerTarget = true;
            //spc.enabled = false;
            UserData.Instance.alertLevel = 2;
            Debug.Log("alert level: " + UserData.Instance.alertLevel);

        }

        if (UserData.Instance.alertBarAmount >= 100)
        {
            sp.playerTarget = true;
            //spc.enabled = true;
            UserData.Instance.alertLevel = 3;
            Debug.Log("Player Has reached max difficulty");
            MaxDifficulty();
        }


        Debug.Log("UserData.Instance.alertBarAmount: " + UserData.Instance.alertBarAmount);
    }

}
