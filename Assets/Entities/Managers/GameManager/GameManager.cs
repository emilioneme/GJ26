using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public AudioSource Scaryaudio;
    public static GameManager Instance { get; set; }

    public float alertBarAmount = 0;
    public int alertLevel = 0;

    public GameObject player;

    public GameObject spotlight;

    public string level;

    SpotlightSpin sp;

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
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sp = spotlight.GetComponent<SpotlightSpin>();
    }

    public void HighAlert()
    {
        if(alertBarAmount < 75)
        {
            alertBarAmount = 75;
        }
    }

    public void MaxDifficulty() 
    {

    }

    public void RaiseAlert(float alertAmount)
    {
        alertBarAmount = Math.Clamp(alertBarAmount + alertAmount, 0, 100);

        if(alertBarAmount < 33 && alertLevel > 0 && !level.Equals("Level2"))
        {
            sp.playerTarget = false;
            //spc.enabled = true;
            alertLevel = 0;
            Debug.Log("alert level: " + alertLevel);
        }


        if ((alertBarAmount > 33 && alertBarAmount < 66 && alertLevel != 1) || level.Equals("Level2") && alertLevel != 1 && alertBarAmount < 66)
        {
            sp.playerTarget = false;
            //spc.enabled = true;
            alertLevel = 1;
            Debug.Log("alert level: " + alertLevel);
        }

        if (alertBarAmount > 66 && alertLevel != 2)
        {
            sp.playerTarget = true;
            //spc.enabled = false;
            alertLevel = 2;
            Debug.Log("alert level: " + alertLevel);

        }

        if (alertBarAmount >= 100)
        {
            sp.playerTarget = true;
            //spc.enabled = true;
            alertLevel = 3;
            Debug.Log("Player Has reached max difficulty");
            MaxDifficulty();
        }


        Debug.Log("alertBarAmount: " + alertBarAmount);
    }

}
