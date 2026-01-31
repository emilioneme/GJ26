using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

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

    public  float health = 10f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log("Player has died.");
        }
    }


}
