using OpenTK.Mathematics;

public class Vertex
{
    private Vector3 position;

    public Vertex(float x, float y, float z)
    {
        position = new Vector3(x, y, z);
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

    public void SetPosition(float x, float y, float z)
    {
        position = new Vector3(x, y, z);
    }
}