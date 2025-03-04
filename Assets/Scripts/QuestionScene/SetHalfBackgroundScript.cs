using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SetHalfBackground : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Cài đặt chiếm 50% màn hình theo chiều ngang
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Giữ nguyên tỉ lệ hình ảnh (aspect ratio)
        if (image != null)
        {
            image.preserveAspect = true; // Giữ tỉ lệ hình
        }
    }
}
