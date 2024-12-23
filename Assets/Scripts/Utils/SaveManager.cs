using UnityEngine;
using System.Collections.Generic;

public static class SaveManager
{

    public static void SaveProgress(int currentCaseIndex, int currentStepIndex)
    {
        PlayerPrefs.SetInt(ConstantStrings.CurrentCaseIndexKey, currentCaseIndex);
        PlayerPrefs.SetInt(ConstantStrings.CurrentStepIndexKey, currentStepIndex);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentCaseIdnex()
    {
        return PlayerPrefs.GetInt(ConstantStrings.CurrentCaseIndexKey, 0);
    }

    public static int LoadCurrentStepIndex()
    {
        return PlayerPrefs.GetInt(ConstantStrings.CurrentStepIndexKey, 0);
    }


    public static void SavePosition(string key, Vector2 position)
    {
        PlayerPrefs.SetFloat(key + "_x", position.x);
        PlayerPrefs.SetFloat(key + "_y", position.y);
        PlayerPrefs.Save();
    }

    public static Vector2 LoadPosition(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_x");
        float y = PlayerPrefs.GetFloat(key + "_y");
        return new Vector2(x, y);
    }

    public static void SaveCompletedCases(List<int> completedCases)
    {
        string completedCasesString = string.Join(",", completedCases);
        PlayerPrefs.SetString(ConstantStrings.CompletedCasesKey, completedCasesString);
        PlayerPrefs.Save();
    }

    public static List<int> LoadCompletedCases()
    {
        string completedCasesString = PlayerPrefs.GetString(ConstantStrings.CompletedCasesKey, "");
        if (string.IsNullOrEmpty(completedCasesString))
        {
            return new List<int>();
        }

        string[] completedCasesArray = completedCasesString.Split(',');
        List<int> completedCases = new List<int>();
        foreach (var caseIndex in completedCasesArray)
        {
            completedCases.Add(int.Parse(caseIndex));
        }

        return completedCases;
    }

    public static void ClearProgress()
    {
        PlayerPrefs.DeleteKey(ConstantStrings.CompletedCasesKey);
        PlayerPrefs.DeleteKey(ConstantStrings.CurrentStepIndexKey);
        PlayerPrefs.DeleteKey(ConstantStrings.CurrentCaseIndexKey);
        PlayerPrefs.Save();
    }
}

public static class ConstantStrings
{
    public const string CurrentCaseIndexKey = "CurrentCaseIndex";
    public const string CurrentStepIndexKey = "CurrentStepIndex";
    public const string CompletedCasesKey = "CompletedCases";
}
