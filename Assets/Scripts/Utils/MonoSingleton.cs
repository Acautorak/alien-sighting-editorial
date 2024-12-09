using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectsByType<T>(FindObjectsSortMode.None)[0];

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private bool _dontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;

            if(_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            Debug.LogWarning("Puko ti je monosingleton: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}