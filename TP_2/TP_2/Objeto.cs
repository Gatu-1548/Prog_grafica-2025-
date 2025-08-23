using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
namespace TP_2
{
    public class Objeto3D
    {
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();
        public List<Face> Caras { get; set; } = new List<Face>();

        // Datos para OpenGL
        private int _vao;
        private int _vbo;
        private float[] _vertexData;

        public void CrearBuffers()
        {
            
            List<float> data = new List<float>();
            foreach (var cara in Caras)
            {
                if (cara.Indices.Count == 4)
                {
                    
                    var v0 = Vertices[cara.Indices[0]];
                    var v1 = Vertices[cara.Indices[1]];
                    var v2 = Vertices[cara.Indices[2]];
                    data.AddRange(new float[] { v0.X, v0.Y, v0.Z, v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z });
                   
                    var v3 = Vertices[cara.Indices[3]];
                    data.AddRange(new float[] { v0.X, v0.Y, v0.Z, v2.X, v2.Y, v2.Z, v3.X, v3.Y, v3.Z });
                }
            }
            _vertexData = data.ToArray();

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Length * sizeof(float), _vertexData, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Dibujar()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexData.Length / 3);
        }
    }
}
