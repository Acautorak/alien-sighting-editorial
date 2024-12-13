using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CaseFileUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("SpriteSwap")]
    [SerializeField] private Sprite upperSprite;
    [SerializeField] private Sprite lowerSprite;
    [SerializeField] private float upperScale = 0.3f;

    [Header("UI components")]

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    [Header("Values")]
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] private float bounceTime = 1f;
    [SerializeField] private float bounceDistance = 300f;
    [SerializeField] private float revealDuration = 2f;
    [SerializeField] private float alphaDuration = 2f;
    private Vector2 startingPosition = new Vector2(0, 400);

    [SerializeField] private bool isDocked = false;

    private void Start()
    {
        NotificationBuss.Subscribe("caseDropped", OnCaseDropped);

        SpriteSwap();

    }
    private void OnDestroy()
    {
        NotificationBuss.Unsubscribe("caseDropped", OnCaseDropped);
    }

    private void OnCaseDropped(object obj)
    {
        SetLockTrue();
        if (obj is bool isLeft && isLeft == true)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }

    }

    private void SpriteSwap()
    {
        if (rectTransform.anchoredPosition.y > 0)
        {
            image.sprite = upperSprite;
            rectTransform.localScale = new Vector3(upperScale, upperScale, 1);
        }
        else
        {
            image.sprite = lowerSprite;
            rectTransform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDocked) return;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDocked) return;

        rectTransform.anchoredPosition += eventData.delta;

        SpriteSwap();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDocked) return;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = rectTransform.anchoredPosition;
    }

    public void SetLockTrue()
    {
        isDocked = true;
        SpriteSwap();
    }

    public void SetLockFalse()
    {
        isDocked = false;
        SpriteSwap();
    }

    private void MoveRight()
    {
        LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x - moveDistance, moveTime)
        .setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + bounceDistance, bounceTime)
            .setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                SetLockFalse();
                gameObject.SetActive(false);
                LeanTween.delayedCall(0.1f, () => { gameObject.SetActive(true); });
            });
        });
    }

    private void MoveLeft()
    {
        LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + moveDistance, moveTime)
        .setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x - bounceDistance, bounceTime)
            .setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                SetLockFalse();
                gameObject.SetActive(false);
                LeanTween.delayedCall(0.1f, () => { gameObject.SetActive(true); });
            });
        });
    }

    private void StartingAnimation()
    {
        LeanTween.scaleX(gameObject, 0.3f, revealDuration);
        LeanTween.scaleY(gameObject, 0.3f, revealDuration)
        .setEase(LeanTweenType.easeOutQuad);
        LeanTween.value(gameObject, 0, 1, alphaDuration)
        .setEase(LeanTweenType.easeOutQuad)
        .setOnUpdate((float val) => { canvasGroup.alpha = val; });
    }

    private void OnEnable()
    {
        rectTransform.SetParent(rectTransform.GetComponentInParent<Canvas>().transform);
        rectTransform.anchoredPosition = startingPosition;
        SpriteSwap();
        SetLockFalse();
        rectTransform.localScale = new Vector3(1f, 0, 1f);
        canvasGroup.alpha = 0f;
        StartingAnimation();
    }



}
