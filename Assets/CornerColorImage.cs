using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CornerColorImage : BaseMeshEffect
{
    [SerializeField]
    Color topLeft;
    public Color TopLeft
    {
        get { return topLeft; }
        set { topLeft = value; this.graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    Color topRight;
    public Color TopRight
    {
        get { return topRight; }
        set { topRight = value; this.graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    Color bottomLeft;
    public Color BottomLeft
    {
        get { return bottomLeft; }
        set { bottomLeft = value; this.graphic.SetVerticesDirty(); }
    }

    [SerializeField]
    Color bottomRight;
    public Color BottomRight
    {
        get { return bottomRight; }
        set { bottomRight = value; this.graphic.SetVerticesDirty(); }
    }

    [SerializeField, Range(0, 5)]
    int divisionCount = 2;

    static List<UIVertex> tempVertexTriangleStream = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!this.IsActive())
            return;

        vh.GetUIVertexStream(tempVertexTriangleStream);

        ModifyVertices(tempVertexTriangleStream);

        vh.Clear();
        vh.AddUIVertexTriangleStream(tempVertexTriangleStream);

        tempVertexTriangleStream.Clear();
    }

    void ModifyVertices(List<UIVertex> vertices)
    {
        var rect = graphic.rectTransform.rect;
        if (rect.width <= 0f || rect.height <= 0f)
            return;

        for (int i = 0; i < divisionCount; i++)
            DivideTriangles(vertices);

        for (int i = 0; i < vertices.Count; i++)
            vertices[i] = GetMultipliedColoredVertex(vertices[i]);
    }

    void DivideTriangles(List<UIVertex> vertices)
    {
        int vertexCount = vertices.Count;

        for (int i = 0; i < vertexCount; i += 3)
        {
            var v0 = vertices[i];
            var v1 = vertices[i + 1];
            var v2 = vertices[i + 2];
            var v01 = AverageTwoVertices(v0, v1);
            var v12 = AverageTwoVertices(v1, v2);
            var v20 = AverageTwoVertices(v2, v0);

            vertices[i + 1] = v01;
            vertices[i + 2] = v20;

            vertices.Add(v1);
            vertices.Add(v12);
            vertices.Add(v01);

            vertices.Add(v2);
            vertices.Add(v20);
            vertices.Add(v12);

            vertices.Add(v01);
            vertices.Add(v12);
            vertices.Add(v20);
        }
    }

    UIVertex AverageTwoVertices(UIVertex v1, UIVertex v2)
    {
        return new UIVertex() {
            color = AverageTwoColors(v1.color, v2.color),
            normal = (v1.normal + v2.normal) / 2f,
            position = (v1.position + v2.position) / 2f,
            tangent = (v1.tangent + v2.tangent) / 2f,
            uv0 = (v1.uv0 + v2.uv0) / 2f,
            uv1 = (v1.uv1 + v2.uv1) / 2f
        };
    }

    Color32 AverageTwoColors(Color32 c1, Color32 c2)
    {
        return new Color32(
            (byte)((c1.r + c2.r) / 2),
            (byte)((c1.g + c2.g) / 2),
            (byte)((c1.b + c2.b) / 2),
            (byte)((c1.a + c2.a) / 2)
        );
    }

    UIVertex GetMultipliedColoredVertex(UIVertex vertex)
    {
        vertex.color = GetMultipliedColor(
            vertex.color,
            GetColorFromCornerColor(vertex.position, graphic.rectTransform.rect)    
        );
        return vertex;
    }

    Color32 GetMultipliedColor(Color32 color1, Color32 color2)
    {
        return new Color32(
            (Byte)(color1.r * color2.r / 255),
            (Byte)(color1.g * color2.g / 255),
            (Byte)(color1.b * color2.b / 255),
            (Byte)(color1.a * color2.a / 255));
    }

    Color32 GetColorFromCornerColor(Vector3 position, Rect rect)
    {
        float x = (position.x - rect.xMin) / rect.width;
        float y = (position.y - rect.yMin) / rect.height;

        return new Color(
            (topLeft.r * (1f - x) + topRight.r * x) * y + (bottomLeft.r * (1f - x) + bottomRight.r * x) * (1f - y),
            (topLeft.g * (1f - x) + topRight.g * x) * y + (bottomLeft.g * (1f - x) + bottomRight.g * x) * (1f - y),
            (topLeft.b * (1f - x) + topRight.b * x) * y + (bottomLeft.b * (1f - x) + bottomRight.b * x) * (1f - y),
            (topLeft.a * (1f - x) + topRight.a * x) * y + (bottomLeft.a * (1f - x) + bottomRight.a * x) * (1f - y)
        );
    }
}
