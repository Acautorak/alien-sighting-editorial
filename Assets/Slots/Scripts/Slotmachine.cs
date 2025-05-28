using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SlotMachineState
{
    Idle,
    Spinning,
    Stopping,
    Landing
}

public class SlotMachine : MonoBehaviour
{
    [Header("Slot Machine Settings")]
    public List<SlotReel> reels;
    public List<GameObject> symbolPrefabs;
    public float spinDuration = 2.0f;

    private Coroutine spinCoroutine;
    private bool isSpinning = false;

    public SlotMachineState State { get; private set; } = SlotMachineState.Idle;

    // Constants
    private const float ReelDelay = 0.3f;
    private const float WinCheckBuffer = 0.2f;
    private static readonly List<int[]> Paylines = new()
    {
        new int[] { 0, 0, 0 },
        new int[] { 1, 1, 1 },
        new int[] { 2, 2, 2 },
        new int[] { 0, 1, 2 },
        new int[] { 2, 1, 0 }
    };

    /// <summary>
    /// Starts the slot machine spin.
    /// </summary>
    public void StartSpin()
    {
        if (State != SlotMachineState.Idle) return;
        State = SlotMachineState.Spinning;
        isSpinning = true;

        for (int i = 0; i < reels.Count; i++)
        {
            float delay = i * ReelDelay;
            reels[i].StartSpin(symbolPrefabs, spinDuration + delay, this);
        }

        RestartSpinCoroutine(spinDuration + reels.Count * ReelDelay + WinCheckBuffer);
    }

    /// <summary>
    /// Coroutine to check for win after a delay.
    /// </summary>
    private IEnumerator CheckWinAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            if (!isSpinning) yield break;
            timer += Time.deltaTime;
            yield return null;
        }
        EvaluateWin();
    }

    /// <summary>
    /// Stops the spin immediately.
    /// </summary>
    public void Slam()
    {
        if (State != SlotMachineState.Spinning) return;
        State = SlotMachineState.Stopping;
        isSpinning = false;
        StopSpinCoroutine();

        foreach (var reel in reels)
        {
            reel.ForceStop(symbolPrefabs);
        }
    }

    /// <summary>
    /// Evaluates all paylines for wins.
    /// </summary>
    private void EvaluateWin()
    {
        foreach (var line in Paylines)
        {
            if (!TryGetSymbolsForLine(line, out string[] symbols)) continue;

            if (AllEqual(symbols))
            {
                Debug.Log($"🎉 WIN on line {string.Join("-", line)}: {symbols[0]} x{reels.Count}");
                // TODO: trigger animation/reward
            }
            else
            {
                Debug.Log($"Line {string.Join("-", line)}: {string.Join(" | ", symbols)} - No match");
            }
        }
    }

    /// <summary>
    /// Checks if all symbols in the array are equal and not empty.
    /// </summary>
    private bool AllEqual(string[] symbols)
    {
        if (symbols.Length == 0) return false;
        string first = symbols[0];
        if (string.IsNullOrEmpty(first)) return false;

        foreach (var s in symbols)
            if (s != first) return false;

        return true;
    }

    /// <summary>
    /// Gets the symbols for a given payline, returns false if invalid.
    /// </summary>
    private bool TryGetSymbolsForLine(int[] line, out string[] symbols)
    {
        symbols = new string[reels.Count];
        for (int i = 0; i < reels.Count; i++)
        {
            int row = line[i];
            if (row >= reels[i].finalSymbols.Count)
                return false;
            symbols[i] = reels[i].finalSymbols[row];
        }
        return true;
    }

    /// <summary>
    /// Called when all reels have landed.
    /// </summary>
    public void OnAllReelsLanded()
    {
        State = SlotMachineState.Idle;
        EvaluateWin();
    }

    /// <summary>
    /// Gets the center row symbols.
    /// </summary>
    private List<string> GetCenterRowSymbols()
    {
        List<string> symbols = new();
        foreach (var reel in reels)
        {
            string symbol = reel.finalSymbols.Count > 1 ? reel.finalSymbols[1] : "";
            symbols.Add(symbol);
        }
        return symbols;
    }

    /// <summary>
    /// Checks if the given symbols are a winning combination.
    /// </summary>
    private bool IsWinningCombination(List<string> symbols)
    {
        return symbols.Count == 3 && symbols[0] == symbols[1] && symbols[1] == symbols[2] && !string.IsNullOrEmpty(symbols[0]);
    }

    /// <summary>
    /// Checks if there are enough reels.
    /// </summary>
    private bool HasEnoughReels()
    {
        return reels.Count >= 3;
    }

    /// <summary>
    /// Handles input for spinning and slamming.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed. State: " + State);
            if (State == SlotMachineState.Idle)
                StartSpin();
            else if (State == SlotMachineState.Spinning)
                Slam();
        }
    }

    /// <summary>
    /// Stops the spin coroutine if running.
    /// </summary>
    private void StopSpinCoroutine()
    {
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
        }
    }

    /// <summary>
    /// Restarts the spin coroutine with a new delay.
    /// </summary>
    private void RestartSpinCoroutine(float delay)
    {
        StopSpinCoroutine();
        spinCoroutine = StartCoroutine(CheckWinAfterDelay(delay));
    }
}
