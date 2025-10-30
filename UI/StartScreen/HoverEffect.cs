using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI�� ���콺 �����͸� �ø� ��, ���̶���Ʈ ȿ���� �ִ� Ŭ����
/// </summary>
public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject hoverImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.transform.SetParent(transform);
            hoverImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            hoverImage.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.SetActive(false);
        }
    }
}
