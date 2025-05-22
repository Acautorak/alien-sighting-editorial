using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    public float symbolHeight = 1.2f;
    public int visibleSymbols = 3;
    public float speed = 10f;

    public AnimationCurve easeCurve;
    public AudioSource audioSource;
    public AudioClip spinClip, stopClip;
    public GameObject stopVFX;
    public SymbolPool pool;

    private List<GameObject> currentSymbols = new();
    private bool isSpinning = false;
    private bool isStopping = false;

    public List<string> finalSymbols = new(); // Used by SlotMachine

    private SlotMachine slotMachine;

    // Slot machine state
    public SlotMachineState State { get; private set; } = SlotMachineState.Idle;

    private Transform symbolContainer;

    private Coroutine spinRoutine;

    void Awake()
    {
        symbolContainer = new GameObject("SymbolContainer").transform;
        symbolContainer.SetParent(transform, false);
    }

    // When starting a spin
    public void StartSpin(List<GameObject> prefabPool, float duration)
    {
        if (State != SlotMachineState.Idle) return;
        State = SlotMachineState.Spinning;
        isSpinning = true;
        isStopping = false;
        PlaySpinSound();

        // If first spin, fill up the reel
        if (currentSymbols.Count == 0)
        {
            for (int i = 0; i < visibleSymbols + 2; i++)
            {
                GameObject symbol = CreateNewSymbol(prefabPool, i * symbolHeight);
                currentSymbols.Add(symbol);
            }
        }

        if (spinRoutine != null) StopCoroutine(spinRoutine);
        spinRoutine = StartCoroutine(SpinRoutine(prefabPool, duration));
    }

    private IEnumerator SpinRoutine(List<GameObject> prefabPool, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && isSpinning)
        {
            float moveAmount = speed * Time.deltaTime;
            symbolContainer.localPosition += Vector3.down * moveAmount;
            RecycleSymbolsIfNeeded(prefabPool);
            elapsed += Time.deltaTime;
            yield return null;
        }
        ForceStop(prefabPool);
    }

    void MoveSymbols(List<GameObject> prefabPool, float moveAmount)
    {
        // Move all symbols down
        foreach (var symbol in currentSymbols)
        {
            symbol.transform.localPosition += Vector3.down * moveAmount;
        }

        // Recycle symbols that have moved off the bottom and add new ones at the top
        for (int i = currentSymbols.Count - 1; i >= 0; i--)
        {
            if (currentSymbols[i].transform.localPosition.y < -symbolHeight)
            {
                pool.Return(currentSymbols[i]);
                currentSymbols.RemoveAt(i);

                // Find the highest Y position
                float maxY = float.MinValue;
                foreach (var s in currentSymbols)
                    if (s.transform.localPosition.y > maxY)
                        maxY = s.transform.localPosition.y;

                GameObject newSymbol = CreateNewSymbol(prefabPool, maxY + symbolHeight);
                currentSymbols.Insert(0, newSymbol);
            }
        }
    }

    private void RecycleSymbolsIfNeeded(List<GameObject> prefabPool)
    {
        // If the bottom symbol is out of view, recycle it to the top
        while (currentSymbols.Count > 0 && 
               currentSymbols[currentSymbols.Count - 1].transform.localPosition.y + symbolContainer.localPosition.y < -symbolHeight)
        {
            var bottom = currentSymbols[currentSymbols.Count - 1];
            pool.Return(bottom);
            currentSymbols.RemoveAt(currentSymbols.Count - 1);

            // Find the highest Y position
            float maxY = float.MinValue;
            foreach (var s in currentSymbols)
                if (s.transform.localPosition.y > maxY)
                    maxY = s.transform.localPosition.y;

            GameObject newSymbol = CreateNewSymbol(prefabPool, maxY + symbolHeight);
            currentSymbols.Insert(0, newSymbol);
        }
    }

    public void ForceStop(List<GameObject> prefabPool)
    {
        if (!isSpinning) return;
        isSpinning = false;
        isStopping = true;
        if (spinRoutine != null) StopCoroutine(spinRoutine);

        // Snap container to zero
        symbolContainer.localPosition = Vector3.zero;

        // Align all symbols to their final positions and animate
        int finished = 0;
        int symbolCount = currentSymbols.Count;
        for (int i = 0; i < symbolCount; i++)
        {
            GameObject symbol = currentSymbols[i];
            float targetY = i * symbolHeight;
            LeanTween.moveLocalY(symbol, targetY, 0.28f)
                .setEase(LeanTweenType.easeOutElastic)
                .setOnComplete(() =>
                {
                    finished++;
                    if (finished == symbolCount)
                    {
                        FinalizeLanding();
                        PlayStopEffect();
                        isStopping = false;
                        OnLandingComplete();
                    }
                });
        }
    }

    GameObject CreateNewSymbol(List<GameObject> prefabPool, float yPos)
    {
        GameObject prefab = prefabPool[Random.Range(0, prefabPool.Count)];
        GameObject symbol = pool.Get(prefab);
        symbol.transform.SetParent(symbolContainer, false);
        symbol.transform.localPosition = new Vector3(0, yPos, 0);

        Symbol symbolTag = symbol.GetComponent<Symbol>();
        symbolTag.symbolName = prefab.name;
        symbolTag.prefabReference = prefab;

        return symbol;
    }

    void PlaySpinSound()
    {
        audioSource?.PlayOneShot(spinClip);
    }

    void PlayStopEffect()
    {
        audioSource?.PlayOneShot(stopClip);
        if (stopVFX)
        {
            Instantiate(stopVFX, transform.position, Quaternion.identity);
        }
    }

    // When starting a spin
    public void StartSpin(List<GameObject> prefabPool, float duration, SlotMachine machine)
    {
        slotMachine = machine;
        if (State != SlotMachineState.Idle) return;
        State = SlotMachineState.Spinning;
        isSpinning = true;
        isStopping = false;
        PlaySpinSound();

        // If first spin, fill up the reel
        if (currentSymbols.Count == 0)
        {
            for (int i = 0; i < visibleSymbols + 2; i++)
            {
                GameObject symbol = CreateNewSymbol(prefabPool, i * symbolHeight);
                currentSymbols.Add(symbol);
            }
        }

        if (spinRoutine != null) StopCoroutine(spinRoutine);
        spinRoutine = StartCoroutine(SpinRoutine(prefabPool, duration));
    }

    // When slamming
    public void Slam(List<GameObject> prefabPool)
    {
        if (State != SlotMachineState.Spinning) return;
        State = SlotMachineState.Stopping;
        ForceStop(prefabPool);
    }

    // When all reels finish landing (call this from the last symbol's OnComplete)
    public void OnLandingComplete()
    {
        State = SlotMachineState.Idle;
        isSpinning = false;

    }

    void AlignSymbolsWithDropAndRise()
    {
        int finished = 0;
        int symbolCount = currentSymbols.Count;

        for (int i = 0; i < symbolCount; i++)
        {
            GameObject symbol = currentSymbols[i]; // Capture reference!
            float targetY = i * symbolHeight;
            float overshootY = targetY - symbolHeight * 0.25f; // Drop below the line

            // Animate drop below, then rise up
            LeanTween.moveLocalY(symbol, overshootY, 0.22f)
                .setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() =>
                {
                    LeanTween.moveLocalY(symbol, targetY, 0.28f)
                        .setEase(LeanTweenType.easeOutElastic)
                        .setOnComplete(() =>
                        {
                            finished++;
                            if (finished == symbolCount)
                            {
                                FinalizeLanding();
                            }
                        });
                });
        }
    }

    void FinalizeLanding()
    {
        finalSymbols.Clear();
        for (int j = 0; j < visibleSymbols; j++)
        {
            if (j < currentSymbols.Count)
            {
                Symbol symbolTag = currentSymbols[j].GetComponent<Symbol>();
                finalSymbols.Add(symbolTag.symbolName);
            }
        }
        // Notify SlotMachine if needed
        if (slotMachine != null)
        {
            bool allIdle = true;
            foreach (var reel in slotMachine.reels)
            {
                if (reel.State != SlotMachineState.Idle)
                {
                    allIdle = false;
                    break;
                }
            }
            if (allIdle)
                slotMachine.OnAllReelsLanded();
        }
    }
}
