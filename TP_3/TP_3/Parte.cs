using System.Collections.Generic;
using OpenTK.Mathematics;

public class Part
{
    private List<Face> faces;
    private Vector3 color;

    public Part(Vector3 color)
    {
        faces = new List<Face>();
        this.color = color;
    }

    public void AddFace(Face f)
    {
        faces.Add(f);
    }

    public List<Face> GetFaces()
    {
        return faces;
    }

    public Vector3 GetColor()
    {
        return color;
    }

    public Vector3 GetCenter()
    {
        Vector3 sum = Vector3.Zero;
        int count = 0;
        foreach (var face in faces)
            foreach (var v in face.GetVertices())
            {
                sum += v.GetPosition();
                count++;
            }
        return count > 0 ? sum / count : Vector3.Zero;
    }
}
