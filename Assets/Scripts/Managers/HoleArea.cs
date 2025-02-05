using UnityEngine;

public class HoleArea : MonoBehaviour
{
    [SerializeField] private RectTransform _holeRect;

    private float _ovalWidth;
    private float _ovalHeight;

    private void Start()
    {
        _ovalWidth = _holeRect.rect.width;
        _ovalHeight = _holeRect.rect.height;
    }

    public bool IsPointInHole(Vector2 screenPoint)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_holeRect, screenPoint, null, out localPoint);
        return (Mathf.Pow(localPoint.x / (_ovalWidth / 2), 2) + Mathf.Pow(localPoint.y / (_ovalHeight / 2), 2)) <= 1;
    }
}
