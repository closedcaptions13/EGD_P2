using UnityEngine;

public static class UIUtil
{
    private static readonly Vector3[] cornersResults = new Vector3[4];
    public static Rect GetWorldBounds(this RectTransform self)
    {
        self.GetWorldCorners(cornersResults);
        return new(cornersResults[0], cornersResults[2] - cornersResults[0]);
    }
}