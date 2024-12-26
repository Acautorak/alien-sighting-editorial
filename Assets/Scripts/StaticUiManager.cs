using UnityEngine;
using UnityEngine.UI;

public class StaticUiManager : MonoSingleton<StaticUiManager>
{
    [Header("References")]
    [SerializeField] private RectTransform bigDocumentRectTransform;
    [SerializeField] private CanvasGroup bigDocumentCanvasGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Image blurryBackgroundImage;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Canvas thisCanvas;
    [SerializeField] private RectTransform content;


    [Header("Properties")]
    [SerializeField] private float moveDuration = 1f;


    private void Start()
    {
        SetDocumentBelowTheScreen();
        playButton.onClick.AddListener(PlayLevel);
        closeButton.onClick.AddListener(CloseDocument);
        NotificationBuss.Subscribe(EventNames.OnCaseButtonClicked, MoveDocumentToCenter);
    }

    private void OnDestroy()
    {
        NotificationBuss.Unsubscribe(EventNames.OnCaseButtonClicked, MoveDocumentToCenter);
    }
    private void SetDocumentBelowTheScreen()
    {
        bigDocumentRectTransform.anchoredPosition = new Vector2(0, -Screen.height - 500f);
        bigDocumentCanvasGroup.interactable = false;
        bigDocumentCanvasGroup.blocksRaycasts = false;
        thisCanvas.enabled = false;

    }

    private void MakeInteractable()
    {
        thisCanvas.enabled = true;
        bigDocumentCanvasGroup.interactable = true;
        bigDocumentCanvasGroup.blocksRaycasts = true;
    }

    public void MoveDocumentToCenter(object obj)
    {
        MakeInteractable();
        FadeToFullBackGround();
        LeanTween.moveY(bigDocumentRectTransform, 0, moveDuration)
        .setEase(LeanTweenType.easeOutQuad)
        .setOnComplete(MakeInteractable);
    }

    private void PlayLevel()
    {
        Debug.Log("pritiso sam play");
    }

    private void CloseDocument()
    {
        bigDocumentCanvasGroup.interactable = false;
        bigDocumentCanvasGroup.blocksRaycasts = false;

        LeanTween.moveY(bigDocumentRectTransform, -Screen.height - 500f, moveDuration)
        .setEase(LeanTweenType.easeOutQuad)
        .setOnComplete(() =>
        {
            thisCanvas.enabled = false;
        });
        FadeBackground();
    }

    private void FadeBackground()
    {
        LeanTween.value(backGroundImage.gameObject, UpdateAlpha, backGroundImage.color.a, 0f, moveDuration)
        .setEase(LeanTweenType.easeInOutQuad);
    }

    private void UpdateAlpha(float alpha)
    {
        Color color = backGroundImage.color;
        color.a = alpha;
        backGroundImage.color = color;
    }

    private void FadeToFullBackGround()
    {
        LeanTween.value(gameObject, UpdateAlpha, 0f, 1f, moveDuration)
        .setEase(LeanTweenType.easeInOutQuad);
    }

    private void SetupSteps()
    {
        foreach (Transform child in content)
        {
             
        }
    }



}
