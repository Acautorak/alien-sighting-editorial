using UnityEngine;

public static class SaveManager
{
    public static void SavePosition(string key, Vector2 position)
    {
        PlayerPrefs.SetFloat(key + "_x", position.x);
        PlayerPrefs.SetFloat(key + "_y", position.y);
        PlayerPrefs.Save();
    }

    public static Vector2 LoadPosition(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_x");
        float y = PlayerPrefs.GetFloat(key + "_y");
        return new Vector2(x, y);
    }
}
