using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;

    [SerializeField] GameObject FadeImageGO;
    [SerializeField] Image FadeImage;


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        volumeSlider.value = UserData.Instance.volume;
        sensitivitySlider.value = UserData.Instance.sensitiviy;
    }

    public void StartGame()
    {
        FadeImageGO.SetActive(true);
        FadeImage.color = Color.clear;
        FadeImage.DOColor(Color.black, 1f)
            .OnComplete(LoadLevel);
    }

    void LoadLevel()
    {
        SceneManager.LoadScene("Level1");
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
