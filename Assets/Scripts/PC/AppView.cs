using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Collections;
using TMPro;

public class AppView : GraphicRaycaster
{
    public Camera screenCamera; // Reference to the camera responsible for rendering the virtual screen's rendertexture

    public GraphicRaycaster screenCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen
    public RectTransform RectTransform => (RectTransform)transform;

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        base.Raycast(eventData, resultAppendList);

        var count = resultAppendList.Count;
        for (var i = count - 1; i >= 0; i--)
        {
            var hit = resultAppendList[i];

            if (hit.gameObject.transform == transform)
            {
                // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
                RectTransform.GetWorldMinMax(out var min, out var max);

                var hitLoc = MathUtil.ReduceDimension(hit.worldPosition, transform.right, transform.up);
                var minLoc = MathUtil.ReduceDimension(min, transform.right, transform.up);
                var maxLoc = MathUtil.ReduceDimension(max, transform.right, transform.up);

                var virtualPos = new Vector3(
                    Mathf.InverseLerp(minLoc.x, maxLoc.x, hitLoc.x),
                    Mathf.InverseLerp(minLoc.y, maxLoc.y, hitLoc.y));

                virtualPos.x *= screenCamera.activeTexture.width;
                virtualPos.y *= screenCamera.activeTexture.height;

                eventData.position = virtualPos;

                //! NOTE !//
                // It is important that you *do not* attempt to
                // append the raycast results of this to the final
                // result list, since doing so introduces z-fighting
                resultAppendList.Clear();
                screenCaster.Raycast(eventData, resultAppendList);

                return;
            }
        }
    }
}