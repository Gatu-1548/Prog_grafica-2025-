using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

class Triangle3D : GameWindow
{
    private int _vbo;
    private int _vao;
    private int _shaderProgram;

    
    private readonly float[] _vertices =
    {
        
         0.0f,  0.5f, 0.0f,   // Arriba
        -0.5f, -0.5f, 0.0f,   // Izquierda
         0.5f, -0.5f, 0.0f    // Derecha
    };

    private Matrix4 _model;
    private Matrix4 _view;
    private Matrix4 _projection;

    public Triangle3D()
        : base(GameWindowSettings.Default,
               NativeWindowSettings.Default)
    {
        Title = "Triángulo 3D con Perspectiva";
        Size = new Vector2i(800, 600);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest); // Muy importante para 3D

        // VBO
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        // VAO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Shaders
        string vertexShaderSource = @"
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
        string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(1.0, 0.5, 0.2, 1.0); // Naranja sólido
            }
        ";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        
        _model = Matrix4.Identity; 
        _view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 2.0f), 
                               Vector3.Zero,                  
                               Vector3.UnitY);                
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);

       
        int modelLoc = GL.GetUniformLocation(_shaderProgram, "model");
        int viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
        int projLoc = GL.GetUniformLocation(_shaderProgram, "projection");

        GL.UniformMatrix4(modelLoc, false, ref _model);
        GL.UniformMatrix4(viewLoc, false, ref _view);
        GL.UniformMatrix4(projLoc, false, ref _projection);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteProgram(_shaderProgram);
        base.OnUnload();
    }

    static void Main()
    {
        using (var window = new Triangle3D())
        {
            window.Run();
        }
    }
}