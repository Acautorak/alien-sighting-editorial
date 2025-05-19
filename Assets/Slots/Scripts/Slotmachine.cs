using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    public List<SlotReel> reels;
    public List<GameObject> symbolPrefabs;
    public float spinDuration = 2.0f;

    private Coroutine spinCoroutine;
    private bool isSpinning = false;

    public void StartSpin()
    {
        isSpinning = true;
        for (int i = 0; i < reels.Count; i++)
        {
            float delay = i * 0.3f;
            reels[i].StartSpin(symbolPrefabs, spinDuration + delay);
        }

        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);
        spinCoroutine = StartCoroutine(CheckWinAfterDelay(spinDuration + reels.Count * 0.3f + 0.2f));
    }

    IEnumerator CheckWinAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            if (!isSpinning) yield break; // Exit if slamming
            timer += Time.deltaTime;
            yield return null;
        }
        EvaluateWin();
    }

    public void Slam()
    {
        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);

        isSpinning = false;
        foreach (var reel in reels)
        {
            reel.ForceStop(symbolPrefabs); // You need to implement this in SlotReel
        }
        EvaluateWin();
    }

    void EvaluateWin()
    {
        List<int[]> paylines = new()
        {
            new int[] { 0, 0, 0 },
            new int[] { 1, 1, 1 },
            new int[] { 2, 2, 2 },
            new int[] { 0, 1, 2 },
            new int[] { 2, 1, 0 }
        };

        foreach (var line in paylines)
        {
            string[] symbols = new string[reels.Count];
            bool validLine = true;

            for (int i = 0; i < reels.Count; i++)
            {
                int row = line[i];
                if (row >= reels[i].finalSymbols.Count)
                {
                    validLine = false;
                    break;
                }

                symbols[i] = reels[i].finalSymbols[row];
            }

            if (!validLine) continue;

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

    bool AllEqual(string[] symbols)
    {
        if (symbols.Length == 0) return false;
        string first = symbols[0];
        if (string.IsNullOrEmpty(first)) return false;

        foreach (var s in symbols)
            if (s != first) return false;

        return true;
    }

    bool HasEnoughReels()
    {
        return reels.Count >= 3;
    }

    List<string> GetCenterRowSymbols()
    {
        List<string> symbols = new List<string>();

        foreach (var reel in reels)
        {
            string symbol = reel.finalSymbols.Count > 1 ? reel.finalSymbols[1] : "";
            symbols.Add(symbol);
        }

        return symbols;
    }

    bool IsWinningCombination(List<string> symbols)
    {
        return symbols.Count == 3 && symbols[0] == symbols[1] && symbols[1] == symbols[2] && !string.IsNullOrEmpty(symbols[0]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSpinning)
            {
                StartSpin();
            }
            else
            {
                Slam();
            }
        }
    }
}
