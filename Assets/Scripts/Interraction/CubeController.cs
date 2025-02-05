using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using UnityEngine.Localization.Settings;

public class CubeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float _jumpPower = 30f;
    [SerializeField] private float _jumpDuration = 0.3f;

    private Vector3 _startPosition;
    private TowerController _towerManager;
    private HoleArea _holeArea;
    private Canvas _canvas;

    public bool IsTowerCube { get; set; }

    public static event Action<string> CubeMessage;

    private void Awake()
    {
        _towerManager = FindObjectOfType<TowerController>();
        _holeArea = FindObjectOfType<HoleArea>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsTowerCube)
        {
            GameObject draggedCube = Instantiate(gameObject, transform.position, Quaternion.identity, _canvas.transform);
            eventData.pointerDrag = draggedCube;
        }
        else
        {
            _startPosition = transform.position;
            transform.parent = _canvas.transform;
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsTowerCube)
        {
            if (_holeArea.IsPointInHole(eventData.position))
            {
                _towerManager.RemoveCube(gameObject);
                DestroyAnimation(gameObject);
                CubeMessage?.Invoke("Cube destroyed in hole");
            }
            else
            {
                transform.DOMove(_startPosition, 0.3f);
                transform.parent = _towerManager.transform;
                CubeMessage?.Invoke("Cube has been returned to tower");
            }
        }
        else
        {
            GameObject draggedCube = eventData.pointerDrag;

            if (_towerManager.IsInTowerArea(eventData.position))
            {
                if (_towerManager.TryAddCube(draggedCube.transform, eventData.position))
                {
                    draggedCube.GetComponent<CubeController>().IsTowerCube = true;
                    draggedCube.transform.DOJump(draggedCube.transform.position, _jumpPower, 1, _jumpDuration);
                    CubeMessage?.Invoke(GetLocalizedString("AddCube"));
                }
                else
                {
                    DestroyAnimation(draggedCube);
                    CubeMessage?.Invoke(GetLocalizedString("Miss"));
                }
            }
            else
            {
                DestroyAnimation(draggedCube);
                CubeMessage?.Invoke(GetLocalizedString("Miss"));
            }
        }
    }

    public void SetTowerBool(bool isTower)
    {
        IsTowerCube = isTower;
    }

    private void DestroyAnimation(GameObject cube)
    {
        cube.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(cube));
    }

    private string GetLocalizedString(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("StringTable", key);
    }
}