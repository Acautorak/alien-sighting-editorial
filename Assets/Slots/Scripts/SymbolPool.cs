using UnityEngine;

public class SymbolPool : MonoBehaviour
{
    public Symbol[] availableSymbols;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Symbol GetRandomSymbol()
    {
        int idx = Random.Range(0, availableSymbols.Length);
        return availableSymbols[idx];
    }
}
