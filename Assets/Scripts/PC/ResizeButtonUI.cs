using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ResizeButtonUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // TODO: make this work? unity UI is evil and nefarious :(


    //! Note !//
    // This object must be at the bottom right of the UI element //

    [SerializeField]
    Vector2 minSizePercentOfOriginal;

    Vector2 originalSize;

    Vector2 dragOffset;
    Vector2 startMin;
    Vector2 startMax;
    Vector2 startTopLeft;
    Vector2 startSizeDelta;

    RectTransform ParentRectTransform => (RectTransform)transform.parent;

    void Awake()
    {
        originalSize = transform.position - transform.parent.position;
    }

    readonly Vector3[] cornersArray = new Vector3[4];

    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentRectTransform.GetWorldCorners(cornersArray);

        startMin = cornersArray[0];
        startTopLeft = cornersArray[1];
        startMax = cornersArray[2];

        dragOffset = startMin - eventData.position;
        startSizeDelta = ParentRectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var targetCorner = dragOffset + eventData.position;
        var minCorner = startTopLeft + originalSize * minSizePercentOfOriginal;

        targetCorner.x = Math.Max(targetCorner.x, minCorner.x);
        targetCorner.y = Math.Min(targetCorner.y, minCorner.y);

        ParentRectTransform.sizeDelta =
            (targetCorner - startMin) / (startMax - startMin)
            * startSizeDelta;

        ParentRectTransform.offsetMin = Vector2.Max(ParentRectTransform.offsetMin, Vector2.zero);
        ParentRectTransform.offsetMax = Vector2.Min(ParentRectTransform.offsetMax, Vector2.zero);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
