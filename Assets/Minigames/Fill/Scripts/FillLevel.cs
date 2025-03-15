using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class FillLevel : ScriptableObject
{
    public int Row;
    public int Col;
    public List<int> Data;
}
