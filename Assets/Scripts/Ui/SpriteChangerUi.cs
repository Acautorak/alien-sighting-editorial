
using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChangerUi : MonoBehaviour
{
    [SerializeField] private Sprite upperSprite;
    [SerializeField] private Sprite lowerSprite;
    [SerializeField] private float upperScale = 0.3f;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    
    private void Start()
    {
        button.onClick.AddListener(() => {Debug.Log("Dodji kuci");});
       // NotificationBuss.Subscribe("itemDropped", OnItemDropped);
    }

    private void OnItemDropped(object obj)
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if(rectTransform.anchoredPosition.y >0)
        {
            image.sprite = upperSprite;
            rectTransform.localScale = new Vector3(upperScale, upperScale,1);
        }
        else
        {
            image.sprite = lowerSprite;
            rectTransform.localScale = Vector3.one;
        }
    }



}
