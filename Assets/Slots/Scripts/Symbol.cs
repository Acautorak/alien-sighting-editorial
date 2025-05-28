using UnityEngine;
using System;

public class Symbol : MonoBehaviour
{
    public string symbolName;
    [HideInInspector] public GameObject prefabReference;

    public event Action<Symbol> OnOffReel;
    public event Action<Symbol> OnOnReel;

    public void TriggerOffReel() => OnOffReel?.Invoke(this);
    public void TriggerOnReel() => OnOnReel?.Invoke(this);
}
