using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TowerController : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private RectTransform _towerArea;
    [SerializeField] private float _cubeHeight = 100f;
    [SerializeField] private float _maxHorizontalOffset = 0.5f;

    private List<Transform> towerCubes = new List<Transform>();

    public static event Action<string> TowerMessage;

    private SaveLoadService saveLoadService;

    [Inject]
    private void Construct(SaveLoadService saveLoadService)
    {
        this.saveLoadService = saveLoadService;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveTower();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadTower();
        }
    }

    public bool TryAddCube(Transform cube, Vector2 dropPosition)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(_towerArea, dropPosition))
        {
            return false;
        }

        if (towerCubes.Count > 0)
        {
            Transform topCube = towerCubes[^1];
            RectTransform topCubeRect = topCube.GetComponent<RectTransform>();

            Vector2 topCubeCenter = topCubeRect.localPosition;
            float topCubeHalfWidth = topCubeRect.rect.width / 2;
            float topCubeHalfHeight = topCubeRect.rect.height / 2;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_towerArea, dropPosition, null, out Vector2 localPoint);

            if (localPoint.y < topCubeCenter.y + topCubeHalfHeight || localPoint.x < topCubeCenter.x - topCubeHalfWidth || localPoint.x > topCubeCenter.x + topCubeHalfWidth)
                return false;

        }

        if (CalculateTotalHeight() + _cubeHeight > _towerArea.rect.height)
        {
            TowerMessage?.Invoke("Maximum high");
            return false;
        }

        Vector2 position = towerCubes.Count == 0 ? GetFirstCubePosition(dropPosition) : GetNextPosition();

        cube.SetParent(_towerArea, false);
        cube.GetComponent<RectTransform>().localPosition = position;
        towerCubes.Add(cube);

        return true;
    }

    public void RemoveCube(GameObject cube)
    {
        int index = towerCubes.IndexOf(cube.transform);
        if (index == -1) return;

        towerCubes.RemoveAt(index);
        AnimateCubesDown(index);
        TowerMessage?.Invoke("Cube removed from tower");
    }

    public bool IsInTowerArea(Vector2 position) => RectTransformUtility.RectangleContainsScreenPoint(_towerArea, position, null);

    private Vector2 GetFirstCubePosition(Vector2 dropPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_towerArea, dropPosition, null, out Vector2 localPoint);

        Rect rect = _towerArea.rect;
        localPoint.x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
        localPoint.y = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

        return localPoint;
    }

    private Vector2 GetNextPosition()
    {
        if (towerCubes.Count == 0)
        {
            TowerMessage?.Invoke("No cubes in Tower");
            return Vector2.zero;
        }

        Transform topCube = towerCubes[^1];
        RectTransform topCubeRect = topCube.GetComponent<RectTransform>();

        float xPositionRandomOffset = UnityEngine.Random.Range(-_maxHorizontalOffset, _maxHorizontalOffset) * topCubeRect.rect.width;

        float newXPosition = topCubeRect.localPosition.x + xPositionRandomOffset;
        float newYPosition = topCubeRect.localPosition.y + topCubeRect.rect.height;

        Rect towerAreaRect = _towerArea.rect;
        newXPosition = Mathf.Clamp(newXPosition, towerAreaRect.xMin, towerAreaRect.xMax);
        newYPosition = Mathf.Clamp(newYPosition, towerAreaRect.yMin, towerAreaRect.yMax);

        return new Vector2(newXPosition, newYPosition);
    }


    private void AnimateCubesDown(int startIndex)
    {
        for (int i = startIndex; i < towerCubes.Count; i++)
        {
            Transform cube = towerCubes[i];
            RectTransform cubeRect = cube.GetComponent<RectTransform>();

            Vector2 targetPos = new Vector2(cubeRect.anchoredPosition.x, cubeRect.anchoredPosition.y - _cubeHeight);

            cubeRect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    private float CalculateTotalHeight() => towerCubes.Count * _cubeHeight;

    public void SaveTower()
    {
        TowerData data = new TowerData
        {
            cubePositions = new List<Vector2>(),
            cubeColors = new List<Color>()
        };

        foreach (Transform cube in towerCubes)
        {
            RectTransform cubeRect = cube.GetComponent<RectTransform>();
            Image cubeImage = cube.GetComponent<Image>();

            data.cubePositions.Add(cubeRect.anchoredPosition);
            data.cubeColors.Add(cubeImage.color);
        }

        saveLoadService.Save(data);
    }

    public void LoadTower()
    {
        TowerData data = saveLoadService.Load();
        if (data == null) return;

        foreach (Transform cube in towerCubes)
        {
            Destroy(cube.gameObject);
        }
        towerCubes.Clear();

        for (int i = 0; i < data.cubePositions.Count; i++)
        {
            GameObject cube = Instantiate(_cubePrefab, gameObject.transform);
            cube.GetComponent<RectTransform>().anchoredPosition = data.cubePositions[i];
            cube.GetComponent<Image>().color = data.cubeColors[i];
            cube.GetComponent<CubeController>().SetTowerBool(true);

            towerCubes.Add(cube.transform);
        }
    }
}