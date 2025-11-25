using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Buff/Buff UI Assets")]
public class BuffUIAssets : ScriptableObject
{
    // List ?????????????
    public List<BuffUIEntry> entries;

    public Sprite GetSprite(string skillName)
    {
        foreach (var e in entries)
        {
            if (e.skillName == skillName)
                return e.sprites;
        }
        return null;
    }
}

[System.Serializable]
public class BuffUIEntry
{
    public string skillName;
    public Sprite sprites;
}