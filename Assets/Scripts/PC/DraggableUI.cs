using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Vector3 dragOffset;
    Vector3 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = Camera.main.WorldToScreenPoint(transform.position);
        dragOffset = startPos - (Vector3)eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var pos = (Vector3)eventData.position + dragOffset;
        transform.position = Camera.main.ScreenToWorldPoint(pos);

        var rectTransform = transform as RectTransform;
        var parentRectTransform = transform.parent as RectTransform;

        var thisBounds = rectTransform.GetWorldBounds();
        var parentBounds = parentRectTransform.GetWorldBounds();

        var offset = Vector3.zero;

        offset += Vector3.Max(Vector2.zero, parentBounds.min - thisBounds.min);
        offset += Vector3.Min(Vector2.zero, parentBounds.max - thisBounds.max);

        transform.position += offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
