using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Properties")]
    [SerializeField] private int metaScore;
    [SerializeField] private int inGameScore;
    [SerializeField] private int currentStep;
    [SerializeField] private int currentStreakCaseFile; 
}
