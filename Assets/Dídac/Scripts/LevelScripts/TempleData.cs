using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Temple", menuName = "Scriptable Objects/Temple")]
public class TempleData : ScriptableObject
{
    public int templeIndex;
    public int numberOfLevels;
}
