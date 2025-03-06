using UnityEngine;


[System.Serializable]
public class StepData
{
    public string clueText;
    public Sprite clueImage;
}

[CreateAssetMenu(fileName = "NewCaseFile", menuName = "Scriptable Objects/CaseFileScriptableObject")]
public class CaseFileScriptableObject : ScriptableObject
{
    [SerializeField] public StepData[] steps = new StepData[8];
}
