using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI에 마우스 포인터를 올릴 시, 하이라이트 효과를 주는 클래스
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
