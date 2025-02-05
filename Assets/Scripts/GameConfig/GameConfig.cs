using UnityEngine;

[CreateAssetMenu(menuName = "Game Config")]
public class GameConfig : ScriptableObject
{
    public int initialCubeCount = 20;
    public Color[] cubeColors;
}