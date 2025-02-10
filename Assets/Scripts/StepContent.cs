using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepContent : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Image imageComponent;

    public void Setup(string text, Sprite sprite)
    {
        if (string.IsNullOrEmpty(text))
        {
            textComponent.gameObject.SetActive(false);
        }
        else
        {
            textComponent.gameObject.SetActive(true);
            textComponent.text = text;
        }

        if (sprite == null)
        {
            imageComponent.gameObject.SetActive(false);
        }
        else
        {
            imageComponent.gameObject.SetActive(true);
            imageComponent.sprite = sprite;
        }

    }
}
