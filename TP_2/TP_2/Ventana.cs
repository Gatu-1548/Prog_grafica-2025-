using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace TP_2
{
    public class Ventana : GameWindow
    {
        private Objeto3D cpu;
        private Objeto3D monitor;
        private Objeto3D teclado;
        private Objeto3D pataCentral;
        private Objeto3D baseIzquierda;
        private Objeto3D baseDerecha;

        private int _shaderProgram;
        private float angulo = 0f;

        private readonly string _vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            void main()
            {
                gl_Position = projection * view * model * vec4(aPosition, 1.0);
            }
        ";

        private readonly string _fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;
            uniform vec3 color;
            void main()
            {
                FragColor = vec4(color,1.0);
            }
        ";

        public Ventana(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);

            
            cpu = Crear(-0.5f, 0.5f, -1.0f, 1.0f, -0.3f, 0.3f);
            cpu.CrearBuffers();

           
            monitor = Crear(-1.0f, 1.0f, -0.5f, 0.5f, -0.05f, 0.05f);
            monitor.CrearBuffers();

           
            teclado = Crear(-1f, -0.5f, -0.2f, -0.1f, -0.7f, 0.7f);
            teclado.CrearBuffers();

            pataCentral = CrearPataCentralMonitor();
            pataCentral.CrearBuffers();

            baseIzquierda = CrearBasePata(-0.15f);
            baseIzquierda.CrearBuffers();

            baseDerecha = CrearBasePata(0.15f);
            baseDerecha.CrearBuffers();

            // --- Compilar shaders ---
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, _vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, _fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shaderProgram);

            // --- Cámara ---
            Matrix4 view = Matrix4.LookAt(new Vector3(4, 3, 6), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref projection);

            int colorLoc = GL.GetUniformLocation(_shaderProgram, "color");

           
            GL.Uniform3(colorLoc, new Vector3(1f, 0f, 0f));
            Matrix4 modelCPU = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angulo)) *
                               Matrix4.CreateTranslation(-2f, 0f, 0f);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref modelCPU);
            cpu.Dibujar();

            
            GL.Uniform3(colorLoc, new Vector3(0f, 0f, 1f)); // azul
            Matrix4 modelMonitor = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angulo)) *
                                   Matrix4.CreateTranslation(0f, 0.1f, 0f); // monitor un poco más abajo
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref modelMonitor);
            monitor.Dibujar();

            
            GL.Uniform3(colorLoc, new Vector3(0f, 0f, 0f)); // negro
            pataCentral.Dibujar();
            baseIzquierda.Dibujar();
            baseDerecha.Dibujar();
            
            GL.Uniform3(colorLoc, new Vector3(0f, 1f, 0f));
            Matrix4 modelTeclado = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angulo)) *
                                    Matrix4.CreateTranslation(0f, -0.5f, 1f);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref modelTeclado);
            teclado.Dibujar();

            angulo += 20f * (float)args.Time;

            SwapBuffers();
        }

        
        private Objeto3D Crear(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        {
            var obj = new Objeto3D();

            obj.Vertices.Add(new Vertex(xMin, yMin, zMin));
            obj.Vertices.Add(new Vertex(xMax, yMin, zMin));
            obj.Vertices.Add(new Vertex(xMax, yMax, zMin));
            obj.Vertices.Add(new Vertex(xMin, yMax, zMin));
            obj.Vertices.Add(new Vertex(xMin, yMin, zMax));
            obj.Vertices.Add(new Vertex(xMax, yMin, zMax));
            obj.Vertices.Add(new Vertex(xMax, yMax, zMax));
            obj.Vertices.Add(new Vertex(xMin, yMax, zMax));

            obj.Caras.Add(new Face(0, 1, 2, 3));
            obj.Caras.Add(new Face(4, 5, 6, 7));
            obj.Caras.Add(new Face(0, 1, 5, 4));
            obj.Caras.Add(new Face(2, 3, 7, 6));
            obj.Caras.Add(new Face(0, 3, 7, 4));
            obj.Caras.Add(new Face(1, 2, 6, 5));

            return obj;
        }

       
        private Objeto3D CrearPataCentralMonitor()
        {
            return Crear(-0.05f, 0.05f, -0.35f, -0.04f, -0.07f, -0.02f); // Y bajado un poco
        }

        // Bases de la pata (más abajo)
        private Objeto3D CrearBasePata(float xOffset)
        {
            return Crear(xOffset - 0.3f, xOffset + 0.3f, -0.45f, -0.35f, -0.12f, -0.07f); // Y bajado
        }
    }
}
