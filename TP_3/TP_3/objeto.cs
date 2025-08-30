using System.Collections.Generic;
using OpenTK.Mathematics;

public class Objeto
{
    private List<Part> parts;
    private Vector3 center;

    public Objeto()
    {
        parts = new List<Part>();
        center = Vector3.Zero;
    }

    // ===== Agregar parte =====
    public void AddPart(Part p)
    {
        parts.Add(p);
        UpdateCenter();
    }

    // ===== Obtener lista de partes =====
    public List<Part> GetParts()
    {
        return parts;
    }

    // ===== Obtener centro =====
    public Vector3 GetCenter()
    {
        return center;
    }

    // ===== Asignar centro manualmente =====
    public void SetCenter(float x, float y, float z)
    {
        center = new Vector3(x, y, z);
    }

    public void SetCenter(Vector3 newCenter)
    {
        center = newCenter;
    }

    // ===== Calcular centro automáticamente según partes =====
    private void UpdateCenter()
    {
        if (parts.Count == 0) return;

        Vector3 sum = Vector3.Zero;
        foreach (var p in parts)
            sum += p.GetCenter();

        center = sum / parts.Count;
    }

    // ===== Obtener todos los vértices como array de floats =====
    public float[] GetVerticesArray()
    {
        List<float> verts = new List<float>();
        foreach (var part in parts)
            foreach (var face in part.GetFaces())
                foreach (var v in face.GetVertices())
                {
                    Vector3 pos = v.GetPosition();
                    verts.Add(pos.X + center.X);
                    verts.Add(pos.Y + center.Y);
                    verts.Add(pos.Z + center.Z);
                }
        return verts.ToArray();
    }

    // ===== Obtener todos los colores como array de floats =====
    public float[] GetColorsArray()
    {
        List<float> colors = new List<float>();
        foreach (var part in parts)
        {
            Vector3 c = part.GetColor();
            foreach (var face in part.GetFaces())
            {
                int vertCount = face.GetVertices().Count;
                for (int i = 0; i < vertCount; i++)
                {
                    colors.Add(c.X);
                    colors.Add(c.Y);
                    colors.Add(c.Z);
                }
            }
        }
        return colors.ToArray();
    }
}