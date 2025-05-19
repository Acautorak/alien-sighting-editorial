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

    public void StartSpin(List<GameObject> prefabPool, float duration)
    {
        if (isSpinning) return;
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

        // Animate all symbols moving down for the duration
        LeanTween.value(gameObject, 0f, duration, duration)
            .setOnUpdate((float t) =>
            {
                float delta = Time.deltaTime;
                MoveSymbols(prefabPool, speed * delta);
            })
            .setOnComplete(() => StopSpinWithTween(prefabPool));
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

    void StopSpinWithTween(List<GameObject> prefabPool)
    {
        if (isStopping) return;
        isStopping = true;

        float decelDuration = 0.7f; // Duration of deceleration phase
        float startSpeed = speed;
        float endSpeed = speed * 0.15f;

        // Deceleration phase
        LeanTween.value(gameObject, startSpeed, endSpeed, decelDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate((float currentSpeed) =>
            {
                MoveSymbols(prefabPool, currentSpeed * Time.deltaTime);
            })
            .setOnComplete(() =>
            {
                // Align phase: gently bounce symbols into place
                for (int i = 0; i < currentSymbols.Count; i++)
                {
                    float targetY = i * symbolHeight;
                    if (i == currentSymbols.Count - 1)
                    {
                        LeanTween.moveLocalY(currentSymbols[i], targetY, 0.35f)
                            .setEase(LeanTweenType.easeOutBounce)
                            .setOnComplete(() =>
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
                                PlayStopEffect();
                                isSpinning = false;
                                isStopping = false;
                            });
                    }
                    else
                    {
                        LeanTween.moveLocalY(currentSymbols[i], targetY, 0.35f).setEase(LeanTweenType.easeOutBounce);
                    }
                }
            });
    }

    public void ForceStop(List<GameObject> prefabPool)
    {
        if (!isSpinning) return;
        isSpinning = false;
        isStopping = true;

        int lastIndex = currentSymbols.Count - 1;
        for (int i = 0; i < currentSymbols.Count; i++)
        {
            float targetY = i * symbolHeight;
            if (i == lastIndex)
            {
                LeanTween.moveLocalY(currentSymbols[i], targetY, 0.18f)
                    .setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(() =>
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
                        PlayStopEffect();
                        isStopping = false;
                    });
            }
            else
            {
                LeanTween.moveLocalY(currentSymbols[i], targetY, 0.18f).setEase(LeanTweenType.easeInQuad);
            }
        }
    }

    GameObject CreateNewSymbol(List<GameObject> prefabPool, float yPos)
    {
        GameObject prefab = prefabPool[Random.Range(0, prefabPool.Count)];
        GameObject symbol = pool.Get(prefab);
        symbol.transform.SetParent(transform, false);
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
}
