using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public List<Reel> reels;
    public SymbolPool symbolPool;
    public int reelsCount = 3;
    public int symbolsPerReel = 3;

    public event Action OnSpinStarted;
    public event Action<List<Symbol[]>> OnSpinEnded;
    public event Action OnSlam;

    private bool isSpinning = false;

    public void Spin()
    {
        if (isSpinning) return;
        isSpinning = true;
        OnSpinStarted?.Invoke();

        foreach (Reel reel in reels)
        {
            reel.StartSpinning(symbolPool, symbolsPerReel);
        }
    }

    public void Stop()
    {
        if (!isSpinning) return;
        foreach (Reel reel in reels)
        {
            reel.StopSpinning();
        }
        Invoke(nameof(EvaluatePaylines), 1.0f); // Wait for reels to stop
    }

    public void Slam()
    {
        if (!isSpinning) return;
        foreach (Reel reel in reels)
        {
            reel.SlamStop();
        }
        OnSlam?.Invoke();
        Invoke(nameof(EvaluatePaylines), 0.1f); // Immediate evaluation
    }

    private void EvaluatePaylines()
    {
        isSpinning = false;
        List<Symbol[]> result = new List<Symbol[]>();
        foreach (Reel reel in reels)
        {
            result.Add(reel.GetVisibleSymbols());
        }
        OnSpinEnded?.Invoke(result);

        // Example: Simple payline evaluation (horizontal line)
        int winCount = 1;
        string firstSymbol = result[0][1].symbolName;
        for (int i = 1; i < reelsCount; i++)
        {
            if (result[i][1].symbolName == firstSymbol)
                winCount++;
        }
        if (winCount == reelsCount)
            Debug.Log("WIN! Payline: " + firstSymbol);
        else
            Debug.Log("No win.");
    }
}
