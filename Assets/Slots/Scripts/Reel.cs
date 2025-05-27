using UnityEngine;
using System.Collections.Generic;

public class Reel : MonoBehaviour
{
    public Transform symbolsParent; // Assign in inspector, where symbols are spawned
    public float symbolHeight = 1.0f; // Height of one symbol
    public float spinSpeed = 5.0f; // Speed at which symbols move down
    public float visibleAreaHeight = 3.0f; // Height of visible area

    private bool isSpinning = false;
    private SymbolPool symbolPool;
    private int symbolsPerReel;

    private List<GameObject> activeSymbols = new List<GameObject>();

    public void StartSpinning(SymbolPool pool, int symbolsCount)
    {
        symbolPool = pool;
        symbolsPerReel = symbolsCount;
        isSpinning = true;
        ClearSymbols();
        SpawnInitialSymbols();
    }

    public void StopSpinning()
    {
        isSpinning = false;
        // Optionally snap symbols to grid here
    }

    public void SlamStop()
    {
        isSpinning = false;
        // Optionally snap symbols to grid here
    }

    private void SpawnInitialSymbols()
    {
        for (int i = 0; i < symbolsPerReel + 2; i++) // +2 for buffer above/below
        {
            SpawnSymbolAtPosition(i);
        }
    }

    private void SpawnSymbolAtPosition(int index)
    {
        Symbol symbolPrefab = symbolPool.GetRandomSymbol();
        GameObject symbolObj = Instantiate(symbolPrefab.gameObject, symbolsParent);
        symbolObj.transform.localPosition = new Vector3(0, symbolHeight * (symbolsPerReel - index), 0);
        activeSymbols.Add(symbolObj);
    }

    private void Update()
    {
        if (isSpinning)
        {
            for (int i = activeSymbols.Count - 1; i >= 0; i--)
            {
                GameObject symbolObj = activeSymbols[i];
                symbolObj.transform.localPosition += Vector3.down * spinSpeed * Time.deltaTime;

                // If symbol is below visible area, pool it and spawn a new one at the top
                if (symbolObj.transform.localPosition.y < -symbolHeight)
                {
                    activeSymbols.RemoveAt(i);
                    Destroy(symbolObj); // Replace with pooling if needed
                    SpawnSymbolAtTop();
                }
            }
        }
    }

    private void SpawnSymbolAtTop()
    {
        float topY = GetTopY();
        Symbol symbolPrefab = symbolPool.GetRandomSymbol();
        GameObject symbolObj = Instantiate(symbolPrefab.gameObject, symbolsParent);
        symbolObj.transform.localPosition = new Vector3(0, topY + symbolHeight, 0);
        activeSymbols.Insert(0, symbolObj);
    }

    private float GetTopY()
    {
        if (activeSymbols.Count == 0) return 0;
        return activeSymbols[0].transform.localPosition.y;
    }

    private void ClearSymbols()
    {
        foreach (var obj in activeSymbols)
        {
            Destroy(obj); // Replace with pooling if needed
        }
        activeSymbols.Clear();
    }

    public Symbol[] GetVisibleSymbols()
    {
        // Return the symbols currently in the visible area (centered)
        List<Symbol> visible = new List<Symbol>();
        foreach (var obj in activeSymbols)
        {
            float y = obj.transform.localPosition.y;
            if (y <= symbolHeight && y >= -symbolHeight * (symbolsPerReel - 1))
            {
                visible.Add(obj.GetComponent<Symbol>());
            }
        }
        return visible.ToArray();
    }
}
