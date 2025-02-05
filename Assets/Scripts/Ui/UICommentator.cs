using DG.Tweening;
using TMPro;
using UnityEngine;

public class UICommentator : MonoBehaviour
{
    [SerializeField] private TMP_Text _commentText;

    private void OnEnable()
    {
        TowerController.TowerMessage += ShowComment;
        CubeController.CubeMessage += ShowComment;
    }

    private void OnDisable()
    {
        TowerController.TowerMessage -= ShowComment;
        CubeController.CubeMessage -= ShowComment;
    }

    private void ShowComment(string message)
    {
        _commentText.text = message;

        _commentText.DOFade(1f, 0.2f).OnComplete(() =>
        {
            _commentText.DOFade(0f, 0.5f).SetDelay(2f);
        });
    }
}
