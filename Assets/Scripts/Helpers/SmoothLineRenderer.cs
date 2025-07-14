using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

//* This was taken from Temperament *//

/// <summary>
/// A custom line renderer intended to reduce the visual errors incurred by using
/// Unity's default line renderer system.
/// 
/// Supports rendering disconnected segments through <seealso cref="SmoothLineRenderer.BreakPoint"/>.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SmoothLineRenderer : MonoBehaviour
{
    /// <summary>
    /// A special point value used to signify that there should be a break within
    /// the line drawn. Equality comparison against this point is invalid, and should
    /// instead be completed with <seealso cref="IsBreakPoint(Vector2)"/>
    /// </summary>
    public static Vector2 BreakPoint => Vector2.positiveInfinity;

    /// <summary>
    /// Check if the provided vector is a break point, as defined by <seealso cref="BreakPoint"/>
    /// </summary>
    public static bool IsBreakPoint(Vector2 point)
        => float.IsInfinity(point.x) && float.IsInfinity(point.y);

    /// <summary>
    /// A helper struct intended to simplify the process of generating the 
    /// smooth line renderer.
    /// </summary>
    public struct LineMeshBuilder
    {
        /// <summary>
        /// The list to populate with vertex positions.
        /// </summary>
        public NativeList<Vector3> Verts { readonly get; set; }
        /// <summary>
        /// The list to populate with triangle indices.
        /// </summary>
        public NativeList<uint> Tris { readonly get; set; }
        /// <summary>
        /// The list to populate with vertex colors.
        /// </summary>
        public NativeList<Color> Colors { readonly get; set; }

        /// <summary>
        /// The width of line to generate.
        /// </summary>
        public float Width { readonly get; set; }
        /// <summary>
        /// The anglular-step in degrees needed for an added fan vertex.
        /// The smaller that this value is, the more vertices will be used
        /// to create smooth corners.
        /// </summary>
        public float AngleForVertex { readonly get; set; }
        /// <summary>
        /// The angle between two line segments below which no corner 
        /// vertices are generated.
        /// </summary>
        public float AngleForDegenerate { readonly get; set; }
        /// <summary>
        /// The color to fill the vertex color buffer with.
        /// </summary>
        public Color Color { readonly get; set; }
        /// <summary>
        /// An offset to subtract from all added vertices.
        /// </summary>
        public Vector3 Offset { readonly get; set; }

        /// <summary>
        /// Helper function to add a triangle to the generated mesh.
        /// </summary>
        private readonly void Tri(uint a, uint b, uint c)
        {
            Tris.Add(a);
            Tris.Add(b);
            Tris.Add(c);
        }

        /// <summary>
        /// Helper function to add a vertex to the generated mesh.
        /// </summary>
        private readonly void Vert(Vector2 v)
        {
            Verts.Add((Vector3)v - Offset);
            Colors.Add(Color);
        }

        /// <summary>
        /// Get the index for which the next vertex will be generated.
        /// </summary>
        private readonly uint NextVertIndex
            => (uint)Verts.Length;

        /// <summary>
        /// Calculate the number of steps needed to create a fan with the given arc angle
        /// and the current meshing settings.
        /// </summary>
        private readonly int AngleSteps(float angle)
            => Mathf.CeilToInt(Mathf.Abs(angle) / AngleForVertex);

        /// <summary>
        /// Generate a new mid-segment connecting two line segments, including the
        /// optional curve joint seperating them.
        /// </summary>
        public readonly void BuildMidSegment(uint l, uint r, Vector2 start, Vector2 end, Vector2 next, out uint newL, out uint newR)
        {
            newL = 0;
            newR = 0;

            var a = end - start;
            var b = next - end;

            var aPerp = Vector2.Perpendicular(a.normalized) * (Width / 2);
            var bPerp = Vector2.Perpendicular(b.normalized) * (Width / 2);

            var angle = Vector2.SignedAngle(a, b);
            var intersect = Vector2.zero;

            // Early-return if handling the degenerate, quasi-linear case //
            if(Mathf.Abs(angle) < AngleForDegenerate)
            {
                var perp = (aPerp + bPerp) / 2;

                newL = NextVertIndex;
                Vert(end - perp);

                newR = NextVertIndex;
                Vert(end + perp);

                Tri(l, r, newR);
                Tri(newR, newL, l);

                return;
            }

            // Phase 1: body of a; +3 verts, +3 tris
            var lVertIndex = NextVertIndex;
            if (angle < -1e-2f)
            {
                intersect = MathUtil.Intersect(start - aPerp, end - aPerp, end - bPerp, next - bPerp);

                newL = NextVertIndex;
                Vert(intersect);
            }
            else
            {
                Vert(end - aPerp);
            }

            // Mid-vertex: always the mid point exactly
            var mVertIndex = NextVertIndex;
            Vert(end);

            // Right-vertex: if angle is right; return the intersection
            // of the right edges of a and b
            var rVertIndex = NextVertIndex;
            if (angle > 1e-2f)
            {
                intersect = MathUtil.Intersect(start + aPerp, end + aPerp, end + bPerp, next + bPerp);

                newR = NextVertIndex;
                Vert(intersect);
            }
            else
            {
                Vert(end + aPerp);
            }

            Tri(r, rVertIndex, mVertIndex);
            Tri(r, mVertIndex, l);
            Tri(l, mVertIndex, lVertIndex);

            // Phase 2: fan
            var fanVertIndex = lVertIndex;
            Vector2 aPerpForFan = -aPerp;

            if(angle < 0)
            {
                fanVertIndex = rVertIndex;
                aPerpForFan *= -1;
            }

            BuildFan(end, aPerpForFan, angle, mVertIndex, ref fanVertIndex);

            // Phase 3: realign; +1 vert, +2 tri
            if(angle < 0)
            {
                var rght = intersect + bPerp * 2;
                var newVertIndex = NextVertIndex;

                Vert(rght);

                Tri(fanVertIndex, newVertIndex, mVertIndex);
                Tri(mVertIndex, newVertIndex, lVertIndex);

                newR = newVertIndex;
            }
            else
            {
                var left = intersect - bPerp * 2;
                var newVertIndex = NextVertIndex;

                Vert(left);

                Tri(fanVertIndex, mVertIndex, newVertIndex);
                Tri(mVertIndex, rVertIndex, newVertIndex);

                newL = newVertIndex;
            }
        }

        /// <summary>
        /// Construct a fan of vertices.
        /// </summary>
        /// <param name="center">The center point of the fan.</param>
        /// <param name="radius">The radius vector to extend from the fan.</param>
        /// <param name="angle">The angle to sweep over.</param>
        /// <param name="anchor">The vertex index to use as the 'anchor point'. This point must already be created.</param>
        /// <param name="fanIndex">The index of the fan points. The first fan point must already be created.</param>
        public readonly void BuildFan(Vector2 center, Vector2 radius, float angle, uint anchor, ref uint fanIndex)
        {
            // Calculate the number of steps //
            var count = AngleSteps(angle);

            // For every step //
            for (int i = 1; i <= count; i++)
            {
                // Create a new vertex with the requested offset //
                var fanAngle = Mathf.Deg2Rad * i * angle / count;

                var rotRadius = MathUtil.Rotate(radius, fanAngle);
                var newVertIndex = NextVertIndex;

                Vert(center + rotRadius);

                // And create a face facing toward the camera (-z) //
                var sideA = Verts[(int)fanIndex] - Verts[(int)newVertIndex];
                var sideB = Verts[(int)anchor] - Verts[(int)fanIndex];

                if(Vector2.SignedAngle(sideA, sideB) < 0)
                {
                    Tri(newVertIndex, fanIndex, anchor);
                }
                else
                {
                    Tri(newVertIndex, anchor, fanIndex);
                }

                fanIndex = newVertIndex;
            }
        }

        /// <summary>
        /// Create a cap for the provided line segment, returning the vertex index of the 
        /// provided leftmost and rightmost points.
        /// </summary>
        public readonly void BuildStartCap(Vector2 start, Vector2 end, out uint newL, out uint newR)
        {
            // Calculate the vector perpendicular to the line //
            var line = end - start;
            var perp = Vector2.Perpendicular(line.normalized) * (Width / 2);

            newL = NextVertIndex;
            Vert(start - perp);

            newR = NextVertIndex;
            Vert(start + perp);

            // Build the fan to form the cap //
            var fanVertIndex = newR;
            BuildFan(start, perp, 180, newL, ref fanVertIndex);
        }

        /// <summary>
        /// Create a cap to finish the provided line segment, connecting to the left and right
        /// indices of the ending vertics previous line segment.
        /// </summary>
        public readonly void BuildEndCap(uint l, uint r, Vector2 start, Vector2 end)
        {
            // Calculate the vector perpendicular to the line //
            var line = end - start;
            var perp = Vector2.Perpendicular(line.normalized) * (Width / 2);

            var lv = NextVertIndex;
            Vert(end - perp);
            var rv = NextVertIndex;
            Vert(end + perp);

            Tri(r, rv, lv);
            Tri(l, r, lv);

            // Build the fan to form the cap //
            var fanVertIndex = lv;
            BuildFan(end, -perp, 180, rv, ref fanVertIndex);
        }
    }

    NativeList<Vector3> verts;
    NativeList<uint> tris;
    NativeList<Color> colors;

    /// <summary>
    /// Fill out the provided mesh with the given data.
    /// </summary>
    private void BuildMeshFromData(IReadOnlyList<Vector2> positions, float width, Color color, Mesh mesh)
    {
        // Special case: less than two points -> empty mesh //
        if(positions.Count < 2)
        {
            mesh.Clear();
            return;
        }

        verts.Clear();
        tris.Clear();
        colors.Clear();

        offset = positions[0];

        // Create a new mesh builder based on the current settings //
        var builder = new LineMeshBuilder
        {
            Verts = verts,
            Tris = tris,
            Colors = colors,

            Width = width,
            AngleForVertex = angleForVertex,
            AngleForDegenerate = angleForDegenerate,

            Color = color,

            Offset = offset
        };

        // Generate segments from the given positions //
        bool drawnCap = false;
        uint l = 0, r = 0;

        for(int i = 1; i < positions.Count; i++)
        {
            var start = positions[i - 1];
            var end = positions[i];
            var next = positions.Count <= i + 1 ? (Vector2?)null : positions[i + 1];

            // Skip "break points" //
            if (IsBreakPoint(start) || IsBreakPoint(end))
                continue;

            // If currently missing the start cap, create it //
            if(!drawnCap)
            {
                builder.BuildStartCap(start, end, out l, out r);
                drawnCap = true;
            }

            // If there is a requirement for a mid segment, create one //
            if (next.HasValue && !IsBreakPoint(next.Value))
            {
                builder.BuildMidSegment(l, r, start, end, next.Value, out l, out r);
            }
            // If at the end of the positions list, or before a breakpoint, create an end cap //
            else
            {
                builder.BuildEndCap(l, r, start, end);
                drawnCap = false;
            }
        }

        mesh.Clear();

        mesh.SetVertices(verts.AsArray());
        mesh.SetIndexBufferParams(tris.Length, IndexFormat.UInt32);
        mesh.SetIndexBufferData(tris.AsArray(), 0, 0, tris.Length);
        mesh.SetColors(colors.AsArray());

        mesh.SetSubMesh(0, new(0, tris.Length, MeshTopology.Triangles));
    }

    #region // Settings //
    [SerializeField]
    private float angleForVertex = 30;
    [SerializeField]
    private float angleForDegenerate = 5;
    [SerializeField]
    private float width = 1;
    [SerializeField]
    private Color color = Color.white;
    private Vector2 offset;
    #endregion

    /// <summary>
    /// The generated mesh.
    /// </summary>
    public Mesh Mesh { get; private set; }

    /// <summary>
    /// The width with which to render the line.
    /// </summary>
    public float Width
    {
        get => width;
        set => width = value;
    }

    /// <summary>
    /// The vertex color with which to render the line.
    /// </summary>
    public Color Color
    {
        get => color;
        set => color = value;
    }

    /// <summary>
    /// Set the list of points to render.
    /// </summary>
    public void BuildFromPoints(IReadOnlyList<Vector2> points)
    {
        BuildMeshFromData(points, Width, Color, Mesh);

        filter.mesh = Mesh;
        transform.position = new(offset.x, offset.y, transform.position.z);
    }

    MeshFilter filter;

    private void Awake()
    {
        Mesh = new Mesh();
        filter = GetComponent<MeshFilter>();

        verts = new(Allocator.Persistent);
        tris = new(Allocator.Persistent);
        colors = new(Allocator.Persistent);
    }

    private void OnDestroy()
    {
        Destroy(Mesh);

        verts.Dispose();
        tris.Dispose();
        colors.Dispose();
    }
}
