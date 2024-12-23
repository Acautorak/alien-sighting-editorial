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
        NotificationBuss.Subscribe(EventNames.OnCaseDropped, OnCaseDropped);

        SpriteSwap();
        button.onClick.AddListener(OnButtonClicked);

    }

    private void OnButtonClicked()
    {
        NotificationBuss.Publish(EventNames.OnCaseButtonClicked);
    }
    private void OnDestroy()
    {
        NotificationBuss.Unsubscribe(EventNames.OnCaseDropped, OnCaseDropped);
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
        if (isDocked)
        {
            image.sprite = upperSprite;
            canvasGroup.alpha = 1f;
            button.interactable = false;
            return;
        }

        if (rectTransform.anchoredPosition.y > 0 + 100)
        {
            if (image.sprite != upperSprite)
            {
                image.sprite = upperSprite;
                rectTransform.localScale = new Vector3(upperScale, upperScale, 1);
            }
            if (button.interactable) button.interactable = false;
        }
        else
        {
            if (image.sprite != lowerSprite)
            {
                image.sprite = lowerSprite;
                rectTransform.localScale = Vector3.one;
            }
            if (!button.interactable) button.interactable = true;
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
        canvasGroup.alpha = 1.0f;
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
        SetLockTrue();
        rectTransform.pivot = new Vector2(0.5f, 1f);
        //LeanTween.scaleX(gameObject, upperScale, revealDuration).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleY(gameObject, upperScale, revealDuration)
        .setEase(LeanTweenType.easeOutQuad).setFrom(0);
        LeanTween.value(gameObject, 0, 1, alphaDuration)
        .setEase(LeanTweenType.easeOutQuad)
        .setOnUpdate((float val) => { canvasGroup.alpha = val; }).setOnComplete(() =>
        {
            StartSmoothPivotChangeAnimation();
            SetLockFalse();
        });
    }

    private void StartSmoothPivotChangeAnimation()
    {
        Vector2 targetPivot = new Vector2(0.5f, 0.5f);
        Vector2 startPivot = rectTransform.pivot;
        Vector2 startingPosition = rectTransform.anchoredPosition;

        LeanTween.value(gameObject, 0, 1, moveTime).setOnUpdate((float t) =>
        {
            rectTransform.pivot = Vector2.Lerp(startPivot, targetPivot, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startingPosition,
            new Vector2(startingPosition.x, startingPosition.y
            - rectTransform.rect.height * (startPivot.y - targetPivot.y) * 2 * upperScale), t);
        });
    }

    private void OnEnable()
    {
        rectTransform.SetParent(rectTransform.GetComponentInParent<Canvas>().transform);
        rectTransform.anchoredPosition = startingPosition;
        SpriteSwap();
        SetLockFalse();
        rectTransform.localScale = new Vector3(upperScale, 0, 1f);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        StartingAnimation();
    }



}
