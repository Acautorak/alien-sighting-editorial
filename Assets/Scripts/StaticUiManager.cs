using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

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
    [SerializeField] private ObjectPool stepPool;
    [SerializeField] private Scrollbar scrollbar;


    [Header("Properties")]
    [SerializeField] private float moveDuration = 1f;

    private int unlockedSteps;
    private int currentCaseIndex;
    private List<int> completedCaseFiles;
    private CaseFileScriptableObject currentCaseFile;



    private void Start()
    {
        SetDocumentBelowTheScreen();
        playButton.onClick.AddListener(PlayLevel);
        closeButton.onClick.AddListener(CloseDocument);
        NotificationBuss.Subscribe(EventNames.OnCaseButtonClicked, MoveDocumentToCenter);

        LoadProgress();
    }

    private void OnDestroy()
    {
        NotificationBuss.Unsubscribe(EventNames.OnCaseButtonClicked, MoveDocumentToCenter);
    }

    private void LoadProgress()
    {
        currentCaseIndex = SaveManager.LoadCurrentCaseIdnex();
        unlockedSteps = SaveManager.LoadCurrentStepIndex() + 1;
        completedCaseFiles = SaveManager.LoadCompletedCases();
    }

    private void SaveProgress()
    {
        SaveManager.SaveProgress(currentCaseIndex, unlockedSteps - 1);
        SaveManager.SaveCompletedCases(completedCaseFiles);
    }

    public void LoadCase(CaseFileScriptableObject caseFile)
    {
        currentCaseFile = caseFile;
        SetupSteps();
        SetupScrollbar();
    }

    private void SetupSteps()
    {
        // clear past steps 
        foreach (Transform child in content)
        {
            stepPool.ReturnPooledObject(child.gameObject);
        }

        // dodaj nove
        foreach (var stepData in currentCaseFile.steps)
        {
            AddStep(stepData.clueText, stepData.clueImage);
        }
    }

    public void AddStep(string clueText, Sprite clueImage)
    {
        GameObject newStep = stepPool.GetPooledObject();
        newStep.transform.SetParent(content, false);
        newStep.SetActive(true);

        if (clueText != null)
        {
            TextMeshProUGUI textComponent = newStep.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = clueText;
        }

        if (clueImage != null)
        {
            Image imageComponent = newStep.GetComponentInChildren<Image>();
            imageComponent.sprite = clueImage;
        }

        // Deactivate step if not unlocked
        if (unlockedSteps <= content.childCount - 1)
        {
            newStep.SetActive(false);
        }
    }

    public void UnlockNextStep()
    {
        if (unlockedSteps < content.childCount)
        {
            GameObject nextStep = content.GetChild(unlockedSteps).gameObject;
            nextStep.SetActive(true);
            CanvasGroup canvasGroup = nextStep.GetComponent<CanvasGroup>();
            if(canvasGroup != null)
            {
                LeanTween.alphaCanvas(canvasGroup, 1f, 1f);
            }
            unlockedSteps++;
            SaveProgress();
            SetupScrollbar();
        }
    }

    public void MarkCaseAsCompleted()
    {
        if(!completedCaseFiles.Contains(currentCaseIndex))
        {
            completedCaseFiles.Add(currentCaseIndex);
            SaveProgress();
        }
    }

    public void RestartGame()
    {
        SaveManager.ClearProgress();

        unlockedSteps = 1;
        completedCaseFiles.Clear();

        foreach(Transform child in content)
        {
            stepPool.ReturnPooledObject(child.gameObject);
        }

        SetupSteps();
        SetupScrollbar();
    }

    private void SetupScrollbar()
    {
        float totoalHeight = unlockedSteps * stepPool.GetObjectPrefab().GetComponent<RectTransform>().sizeDelta.y;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totoalHeight);

        scrollbar.numberOfSteps = unlockedSteps;
        scrollbar.size = 1f/ unlockedSteps;
        scrollbar.value = 0f;
        
    }
    private void SetDocumentBelowTheScreen()
    {
        bigDocumentRectTransform.anchoredPosition = new Vector2(0, -Screen.height - 500f);
        bigDocumentCanvasGroup.interactable = false;
        bigDocumentCanvasGroup.blocksRaycasts = false;
        thisCanvas.enabled = false;
    }

    // guess its wednesday
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
        UnlockNextStep();
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
}
