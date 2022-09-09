using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct LevelListEntry {
    public string name;

    public string displayName;
    public string sentence;

    [Space(4)]
    public Vector3 playerPosition;
    public GameObject levelPrefab;

    [Space(4)]
    public string musicName;
};

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/Level Data"), System.Serializable]
public class LevelList : ScriptableObject
{
    public string startingLevel;
    public LevelListEntry[] levels;


    public LevelListEntry LoadLevel(string name)
    {
        for(int it_index = 0; it_index < levels.Length; it_index += 1)
        {
            if(levels[it_index].levelPrefab != null && name == levels[it_index].name)
            {
                return levels[it_index];
            }
        }
        return new LevelListEntry();
    }
}
