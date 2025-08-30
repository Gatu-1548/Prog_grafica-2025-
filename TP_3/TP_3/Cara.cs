using System.Collections.Generic;
using OpenTK.Mathematics;

public class Face
{
    private List<Vertex> vertices;
    private Vector3 center;

    public Face()
    {
        vertices = new List<Vertex>();
        center = Vector3.Zero;
    }

    public void AddVertex(Vertex v)
    {
        vertices.Add(v);
        UpdateCenter();
    }

    public List<Vertex> GetVertices()
    {
        return vertices;
    }

    public Vector3 GetCenter()
    {
        return center;
    }

    private void UpdateCenter()
    {
        if (vertices.Count == 0) return;

        Vector3 sum = Vector3.Zero;
        foreach (var v in vertices)
            sum += v.GetPosition();

        center = sum / vertices.Count;
    }
}
