using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        volumeSlider.value = UserData.Instance.volume;
        sensitivitySlider.value = UserData.Instance.sensitiviy;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SetVolume() 
    {
        UserData.Instance.SetVolume(volumeSlider.value);
    }

    public void SetSensitivity()
    {
        UserData.Instance.sensitiviy = sensitivitySlider.value;
    }

}
