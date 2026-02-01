using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public  float health = 10f;
    public int numberOfBacthes = 20;

    public float alertBarAmount = 0;
    public int alertLevel = 0;

    public GameObject player;

    public GameObject spotlight;

    public string level;

    SpotlightSpin sp;

    public GameObject BridgeBlocker;
    public Transform EndingTransform;
    AudioSource watchedAudio;



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
        watchedAudio = spotlight.transform.GetComponent<AudioSource>();


        

    }

    public void PlayWatchedAudio()
    {
        if (!watchedAudio.isPlaying)
        {
            watchedAudio.Play();
        }
    }

    public void StopWatchedAudio()
    {
        if (watchedAudio.isPlaying)
        {
            watchedAudio.Stop();
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log("Player has died.");
        }
    }

    public void HighAlert()
    {
        if(alertBarAmount < 75)
        {
            alertBarAmount = 75;
        }
    }


    public void RaiseAlert(float alertAmount)
    {
        alertBarAmount = Math.Clamp(alertBarAmount + alertAmount, 0, 100);

        if((alertBarAmount > 33 && alertBarAmount < 66 && alertLevel != 1) || level.Equals("Level2") && alertLevel != 1 && alertBarAmount < 66)
        {
            sp.SetPlayerTarget(false);
            sp.spotlightTarget.GetComponent<SpotlightCollider>().enabled = true;
            alertLevel = 1;
            Debug.Log("alert level: " + alertLevel);
        }
        if(alertBarAmount > 66 && alertLevel != 2)
        {
            sp.SetPlayerTarget(true);
            sp.spotlightTarget.GetComponent<SpotlightCollider>().enabled = false;
            alertLevel = 2;
            Debug.Log("alert level: " + alertLevel);

        }
        if(alertBarAmount < 33 && alertLevel > 0 && !level.Equals("Level2"))
        {
            sp.SetPlayerTarget(false);
            sp.spotlightTarget.GetComponent<SpotlightCollider>().enabled = true;
            alertLevel = 0;
            Debug.Log("alert level: " + alertLevel);
        }

        if(alertBarAmount >= 100)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log("Player has died.");
        }

        Debug.Log("alertBarAmount: " + alertBarAmount);
    }

}
