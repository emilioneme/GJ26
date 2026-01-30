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
        volumeSlider.value = UserData.Instance.volume;
        sensitivitySlider.value = UserData.Instance.sensitiviy;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SetVolume() 
    {
        UserData.Instance.volume = volumeSlider.value;
    }

    public void SetSensitivity()
    {
        UserData.Instance.sensitiviy = sensitivitySlider.value;
    }

}
