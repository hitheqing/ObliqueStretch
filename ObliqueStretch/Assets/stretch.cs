using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class stretch : MonoBehaviour
{
    public Mesh       Mesh;
    public MeshFilter MeshFilter;
    public float      a;
    public float      b;
    public float      trw;
    public float      trh;
    public float      brw;
    public float      brh;
    public bool       adjustOrigin = true;

    public void DoTest()
    {
        this.Mesh = CreateSlicedMesh(a: a, b: b, trw: trw, trh: trh, brw: brw, brh: brh, adjustOrigin: adjustOrigin);
        MeshFilter.mesh = Mesh;
    }

    private void OnValidate()
    {
        DoTest();
    }

    private static Mesh CreateSlicedMesh(float w            = 200,  float h   = 100,
                                         float a            = 0.1f, float b   = 0.05f, float trw = 400,
                                         float trh          = 200,  float brw = 200,   float brh = 200,
                                         bool  adjustOrigin = true)
    {
        // pixel to unit
        w   *= 0.01f;
        h   *= 0.01f;
        trw *= 0.01f;
        trh *= 0.01f;
        brw *= 0.01f;
        brh *= 0.01f;

        // 在实际coding中用下标写起来会比较方便
        var yc  = (1 - a - 0.5f) * (1 - b) / (1 - a);
        var xc  = (1 - b - 0.5f) * (1 - a) / (1 - b);
        var x0  = new Vector2(0, 0);
        var x1  = new Vector2(0, 1);
        var x2  = new Vector2(1, 1);
        var x3  = new Vector2(1, 0);
        var x4  = new Vector2(a, 0);
        var x5  = new Vector2(1 - a, 0);
        var x6  = new Vector2(a, 1);
        var x7  = new Vector2(1 - a, 1);
        var x8  = new Vector2(0, 1 - b);
        var x9  = new Vector2(0, b);
        var x10 = new Vector2(1, 1 - b);
        var x11 = new Vector2(1, b);
        var x12 = new Vector2(0.5f, yc);
        var x13 = new Vector2(xc, 0.5f);
        var x14 = new Vector2(0.5f, 1 - yc);
        var x15 = new Vector2(1 - xc, 0.5f);

        var mesh     = new Mesh();
        var vertices = new List<Vector3>();
        var uv       = new List<Vector2>();
        var indices  = new List<int>();

        // 设实际图片大小为 w*h，右上角方向拉伸距离为trw*trh, 右下角方向拉伸距离为brw*brh
        var scale  = new Vector2(w, h);
        var vert0  = x0.scale(scale).add(new Vector3(0, 0, 0));
        var vert1  = x1.scale(scale).add(new Vector3(0, 0, 0));
        var vert2  = x2.scale(scale).add(new Vector3(trw, trh, 0));
        var vert3  = x3.scale(scale).add(new Vector3(brw, brh, 0));
        var vert4  = x4.scale(scale).add(new Vector3(brw, brh, 0));
        var vert5  = x5.scale(scale).add(new Vector3(brw, brh, 0));
        var vert6  = x6.scale(scale).add(new Vector3(trw, trh, 0));
        var vert7  = x7.scale(scale).add(new Vector3(trw, trh, 0));
        var vert8  = x8.scale(scale).add(new Vector3(0, 0, 0));
        var vert9  = x9.scale(scale).add(new Vector3(0, 0, 0));
        var vert10 = x10.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));
        var vert11 = x11.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));
        var vert12 = x12.scale(scale).add(new Vector3(brw, brh, 0));
        var vert13 = x13.scale(scale).add(new Vector3(0, 0, 0));
        var vert14 = x14.scale(scale).add(new Vector3(trw, trh, 0));
        var vert15 = x15.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));

        // move origin point
        if (adjustOrigin) {
            var vec = vert0 - (vert12 + vert14) / 2;

            vert0  += vec;
            vert1  += vec;
            vert2  += vec;
            vert3  += vec;
            vert4  += vec;
            vert5  += vec;
            vert6  += vec;
            vert7  += vec;
            vert8  += vec;
            vert9  += vec;
            vert10 += vec;
            vert11 += vec;
            vert12 += vec;
            vert13 += vec;
            vert14 += vec;
            vert15 += vec;
        }

        vertices.Add(vert0);
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert3);
        vertices.Add(vert4);
        vertices.Add(vert5);
        vertices.Add(vert6);
        vertices.Add(vert7);
        vertices.Add(vert8);
        vertices.Add(vert9);
        vertices.Add(vert10);
        vertices.Add(vert11);
        vertices.Add(vert12);
        vertices.Add(vert13);
        vertices.Add(vert14);
        vertices.Add(vert15);

        uv.Add(x0);
        uv.Add(x1);
        uv.Add(x2);
        uv.Add(x3);
        uv.Add(x4);
        uv.Add(x5);
        uv.Add(x6);
        uv.Add(x7);
        uv.Add(x8);
        uv.Add(x9);
        uv.Add(x10);
        uv.Add(x11);
        uv.Add(x12);
        uv.Add(x13);
        uv.Add(x14);
        uv.Add(x15);

        indices.AddRange(new[] {
            0, 9, 4,
            8, 1, 6,
            7, 2, 10,
            11, 3, 5,

            9, 8, 13,
            6, 7, 14,
            10, 11, 15,
            5, 4, 12,

            4, 9, 13,
            13, 12, 4,
            13, 8, 6,
            6, 14, 13,
            14, 7, 15,
            7, 10, 15,
            15, 11, 5,
            15, 5, 12,

            12, 13, 14,
            12, 14, 15
        });

        mesh.vertices  = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv        = uv.ToArray();
        return mesh;
    }
}


public static class Ex
{
    public static Vector2 scale(this Vector2 lhs, Vector2 rhs)
    {
        return Vector2.Scale(lhs, rhs);
    }

    public static Vector3 add(this Vector2 vector2, Vector3 vector3)
    {
        return new Vector3(vector2.x + vector3.x, vector2.y + vector3.y, vector3.z);
    }
}