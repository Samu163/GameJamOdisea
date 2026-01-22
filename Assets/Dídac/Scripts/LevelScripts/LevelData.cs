using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public int templeIndex;
}
