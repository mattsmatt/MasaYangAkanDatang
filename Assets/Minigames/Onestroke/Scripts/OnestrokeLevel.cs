using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class OnestrokeLevel : ScriptableObject
{
    public int Row, Col;
    public List<Vector4> Points;
    public List<Vector2Int> Edges;
}
