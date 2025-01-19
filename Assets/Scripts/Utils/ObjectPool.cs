using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int initialPoolSize = 2;

    private List<GameObject> pool;

    private void Awake()
    {
        pool = new List<GameObject>();

        for(int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach(GameObject obj in pool)
        {
            if(!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(objectPrefab);
        newObj.SetActive(false);
        pool.Add(newObj);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnPooledObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public GameObject GetObjectPrefab()
    {
        return objectPrefab;
    }
}
