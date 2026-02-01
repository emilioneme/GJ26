using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BlinkStart : MonoBehaviour
{
    [SerializeField] public TMP_Text flashingText;
    void Update()
    {
        var color = flashingText.color;
        color.a = Mathf.Abs(Mathf.Sin(Time.time));
        flashingText.color = color;
    }

}
