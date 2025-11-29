using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Buff/Buff UI Assets")]

[System.Serializable]
public class BuffUIEntry
{
    public string skillName;
    public Sprite sprites;
    public GameObject edgeScreenEffect;
}

public class BuffUIAssets : ScriptableObject
{
    // List of each Buff (Name + Sprite)
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
    
    public BuffUIEntry GetEntry(string skillName)
    {
        foreach (var e in entries)
        {
            if (e.skillName == skillName)
                return e;
        }
        return null;
    }
}