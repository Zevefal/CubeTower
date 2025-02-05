using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform _content;

    private void Start()
    {
        InitializeCubes();
    }

    private void InitializeCubes()
    {
        for (int i = 0; i < gameConfig.initialCubeCount; i++)
        {
            GameObject cube = Instantiate(cubePrefab, _content);
            var imageComponent = cube.GetComponent<UnityEngine.UI.Image>();
            imageComponent.color = gameConfig.cubeColors[i % gameConfig.cubeColors.Length];
        }
    }
}