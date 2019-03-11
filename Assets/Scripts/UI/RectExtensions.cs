using UnityEngine;

/// <summary>
/// Some handy extensions for the unityUI RectTransform class
/// </summary>
public static class RectTransformExtensions
{
    public static void SetWidth(this RectTransform rectTransform, float width)
    {
        rectTransform.sizeDelta = new Vector2(width, rectTransform.rect.height);
    }

    public static void SetHeight(this RectTransform rectTransform, float height)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, height);
    }

    public static void SetX(this RectTransform rectTransform, float x)
    {
        rectTransform.localPosition = new Vector3(x, rectTransform.localPosition.y, rectTransform.localPosition.z);
    }

    public static void SetY(this RectTransform rectTransform, float y)
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, y, rectTransform.localPosition.z);
    }

    public static void SetPosition(this RectTransform rectTransform, float x, float y)
    {
        rectTransform.localPosition = new Vector3(x, y, rectTransform.localPosition.z);
    }

    public static void SetLeft(this RectTransform rectTransform, float left)
    {
        rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
    }

    public static void SetRight(this RectTransform rectTransform, float right)
    {
        rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
    }

    public static void SetTop(this RectTransform rectTransform, float top)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rectTransform, float bottom)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
    }
}
