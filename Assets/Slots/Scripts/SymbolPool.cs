using System.Collections.Generic;
using UnityEngine;

public class SymbolPool : MonoBehaviour
{
    // Pool for each prefab type
    private Dictionary<GameObject, Queue<GameObject>> poolDict = new();

    // Get a pooled symbol or instantiate if none available
    public GameObject Get(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        if (poolDict[prefab].Count > 0)
        {
            GameObject obj = poolDict[prefab].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return Instantiate(prefab);
        }
    }

    // Return a symbol to the pool
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        // Find the prefab this object was created from
        var symbol = obj.GetComponent<Symbol>();
        if (symbol != null && symbol.prefabReference != null)
        {
            if (!poolDict.ContainsKey(symbol.prefabReference))
                poolDict[symbol.prefabReference] = new Queue<GameObject>();
            poolDict[symbol.prefabReference].Enqueue(obj);
        }
        else
        {
            Destroy(obj); // fallback if prefab reference is missing
        }
    }
}
