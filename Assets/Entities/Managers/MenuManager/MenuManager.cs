using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using TMPro;
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

    [SerializeField] TMP_Text InrtoText;

    [SerializeField][TextArea] string introText;
    [SerializeField][TextArea] string outroText;

    [SerializeField] float textDuration;
    [SerializeField] float text2Duration;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        volumeSlider.value = UserData.Instance.volume;
        sensitivitySlider.value = UserData.Instance.sensitiviy;

        StartMenu();
    }


    public void StartMenu() 
    {
        StartCoroutine(TextAppearCoroutine(outroText));
    }

    IEnumerator TextAppearCoroutine(string text)
    {
        FadeImageGO.SetActive(true);
        InrtoText.text = "";
        FadeImage.color = Color.black;

        InrtoText.text = string.Empty;
        for (int i = 0; i < text.Length; i++)
        {
            InrtoText.text += text[i];
            yield return new WaitForSeconds(textDuration);
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(TextDissapearCoroutine(text));
    }

    IEnumerator TextDissapearCoroutine(string text)
    {
        string currentText = InrtoText.text;

        while (currentText.Length > 0)
        {
            currentText = currentText.Remove(currentText.Length - 1);
            InrtoText.text = currentText;
            yield return new WaitForSeconds(textDuration/3);
        }

        InrtoText.text = "";
        FadeImage.color = Color.black;
        FadeImage.DOColor(Color.clear, 1f)
            .OnComplete(()=>FadeImageGO.SetActive(false));
    }


    public void StartGame()
    {
        FadeImageGO.SetActive(true);
        InrtoText.text = "";
        FadeImage.color = Color.clear;
        FadeImage.DOColor(Color.black, 1f)
            .OnComplete(IntroText);
    }

    void IntroText() 
    {
        StartCoroutine(IntroTextAppearCoroutine(introText));
    }

    IEnumerator IntroTextAppearCoroutine(string text)
    {
        InrtoText.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            InrtoText.text += text[i];
            yield return new WaitForSeconds(text2Duration);
        }

        yield return new WaitForSeconds(1);
        InrtoText.text = "";
        LoadLevel();
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
