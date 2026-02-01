using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public  float health = 10f;
    public int numberOfBacthes = 20;

    public float alertBarAmount = 0;
    public int alertLevel = 0;




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


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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


    public void RaiseAlert(float alertAmount)
    {
        alertBarAmount += alertAmount;

        if(alertBarAmount > 33 && alertLevel < 66)
        {
            alertLevel = 1;
            Debug.Log("alert level: " + alertLevel);
        }
        if(alertBarAmount > 66)
        {
            alertLevel = 2;
            Debug.Log("alert level: " + alertLevel);
        }
        if(alertBarAmount < 33 && alertLevel > 0)
        {
            alertLevel = 0;
            Debug.Log("alert level: " + alertLevel);
        }
    }

}
