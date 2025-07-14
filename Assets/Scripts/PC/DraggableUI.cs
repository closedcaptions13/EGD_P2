using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Vector2 dragOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = (Vector2)transform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var rectTransform = (RectTransform)transform;
        rectTransform.position = dragOffset + eventData.position;

        var minClamped = Vector2.Min(
            Vector2.Max(rectTransform.offsetMin, Vector2.zero),
            Vector2.one - rectTransform.sizeDelta
        );

        var offset = minClamped - rectTransform.offsetMin;

        rectTransform.offsetMin += offset;
        rectTransform.offsetMax += offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
