using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoSingleton<CaseManager>
{
    [SerializeField] private string caseFileFolder = "Cases";

    [SerializeField] private List<CaseFileScriptableObject> allCaseFiles = new List<CaseFileScriptableObject>();
    [SerializeField] private List<CaseFileScriptableObject> unsolvedCaseFiles = new List<CaseFileScriptableObject>();
    [SerializeField] private List<int> completedCases;
    [SerializeField] private CaseFileScriptableObject currentCase;

    private void Start()
    {
        LoadAllCaseFiles();
        LoadProgress();
    }

    private void LoadAllCaseFiles()
    {
        CaseFileScriptableObject[] caseFiles = Resources.LoadAll<CaseFileScriptableObject>(caseFileFolder);
        allCaseFiles.AddRange(caseFiles);
    }

    private void LoadProgress()
    {
        completedCases = SaveManager.LoadCompletedCases();
        FilterUnsolvedCases();
    }

    private void FilterUnsolvedCases()
    {
        foreach (CaseFileScriptableObject caseFile in allCaseFiles)
        {
            int caseIndex = allCaseFiles.IndexOf(caseFile);
            if (!completedCases.Contains(caseIndex))
            {
                unsolvedCaseFiles.Add(caseFile);
            }
        }
    }

    private void AssignNewCase()
    {
        if (unsolvedCaseFiles.Count > 0)
        {
            int randomIndex = Random.Range(0, unsolvedCaseFiles.Count);
            CaseFileScriptableObject newCase = unsolvedCaseFiles[randomIndex];
            currentCase = newCase;
        }
        else
        {
            Debug.Log("Resio si sve slucajeve");
            SaveManager.ClearProgress();
        }
    }

    private void MarkCaseAsComplete(CaseFileScriptableObject caseFile)
    {
        int caseIndex = allCaseFiles.IndexOf(caseFile);
        if(!completedCases.Contains(caseIndex))
        {
            completedCases.Add(caseIndex);
            SaveManager.SaveCompletedCases(completedCases);
        }
    }



}
