using UnityEngine;
using System.Collections.Generic;

public static class SaveManager
{

    public static void SaveProgress(int currentCaseIndex, int currentStepIndex)
    {
        PlayerPrefs.SetInt("currentCaseIndex", currentCaseIndex);
        PlayerPrefs.SetInt("currentStepIndex", currentStepIndex);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentCaseIdnex()
    {
        return PlayerPrefs.GetInt("currentCaseIndex", 0);
    }

    public static int LoadCurrentStepIndex()
    {
        return PlayerPrefs.GetInt("currentStepIndex",0);
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
        PlayerPrefs.SetString("completedCases", completedCasesString);
        PlayerPrefs.Save();
    }
}
