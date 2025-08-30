using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

public class TP3Game : GameWindow
{
    private int _shaderProgram;
    private List<Objeto> objetos = new List<Objeto>();
    private Dictionary<Objeto, int> vaos = new Dictionary<Objeto, int>();
    private Dictionary<Objeto, int> vbos = new Dictionary<Objeto, int>();
    private Dictionary<Objeto, int> colorVBOs = new Dictionary<Objeto, int>();

    string vertexShaderSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPos;
        layout(location = 1) in vec3 aColor;
        out vec3 vertexColor;
        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;
        void main()
        {
            gl_Position = projection * view * model * vec4(aPos, 1.0);
            vertexColor = aColor;
        }";

    string fragmentShaderSrc = @"
        #version 330 core
        in vec3 vertexColor;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(vertexColor, 1.0);
        }";

    public TP3Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1f);
        GL.Enable(EnableCap.DepthTest);

        _shaderProgram = CreateShaderProgram(vertexShaderSrc, fragmentShaderSrc);

        // ===== Crear objetos directamente usando tus clases =====
        objetos.Add(CrearPC(-3f, 0f, 0f));
        objetos.Add(CrearMonitor(0f, 0f, 0f));
        objetos.Add(CrearTeclado(0f, -1f, 1.5f));

        foreach (var obj in objetos)
        {
            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();
            int colorVBO = GL.GenBuffer();
            vaos[obj] = vao;
            vbos[obj] = vbo;
            colorVBOs[obj] = colorVBO;

            GL.BindVertexArray(vao);

            // Vertices
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            float[] vertices = obj.GetVerticesArray();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Colores
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVBO);
            float[] colors = obj.GetColorsArray(); // añadimos este método en Objeto
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
        }
    }

    protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(_shaderProgram);

        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
        Matrix4 view = Matrix4.LookAt(new Vector3(8, 6, 12), Vector3.Zero, Vector3.UnitY);

        //esto segun puede estar en el onload
        int projLoc = GL.GetUniformLocation(_shaderProgram, "projection");
        int viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
        int modelLoc = GL.GetUniformLocation(_shaderProgram, "model");

        GL.UniformMatrix4(projLoc, false, ref projection);
        GL.UniformMatrix4(viewLoc, false, ref view);

        foreach (var obj in objetos)
        {
            GL.BindVertexArray(vaos[obj]);
            Matrix4 model = Matrix4.CreateTranslation(obj.GetCenter());
            GL.UniformMatrix4(modelLoc, false, ref model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, obj.GetVerticesArray().Length / 3);
        }

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        foreach (var vbo in vbos.Values) GL.DeleteBuffer(vbo);
        foreach (var colorVBO in colorVBOs.Values) GL.DeleteBuffer(colorVBO);
        foreach (var vao in vaos.Values) GL.DeleteVertexArray(vao);
        GL.DeleteProgram(_shaderProgram);
        base.OnUnload();
    }

    private int CreateShaderProgram(string vertexSrc, string fragmentSrc)
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSrc);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSrc);
        GL.CompileShader(fragmentShader);

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }

    // ===== Métodos para crear los objetos =====
    private Objeto CrearPC(float x, float y, float z)
    {
        Objeto pc = new Objeto();
        pc.AddPart(CrearCuboide(2f, 4f, 2f, new Vector3(0.6f, 0.6f, 0.6f))); // gris metálico
        pc.AddPart(CrearCuboide(1f, 0.2f, 0.1f, new Vector3(0.8f, 0.1f, 0.1f), 0, 1f, -1.05f)); // DVD rojo
        pc.AddPart(CrearCuboide(0.2f, 0.2f, 0.1f, new Vector3(0.1f, 0.1f, 0.8f), 0.7f, -1.5f, -1.05f)); // botón azul
        pc.SetCenter(x, y, z);
        return pc;
    }

    private Objeto CrearMonitor(float x, float y, float z)
    {
        Objeto monitor = new Objeto();
        monitor.AddPart(CrearCuboide(3f, 2f, 0.2f, new Vector3(0.1f, 0.1f, 0.1f))); // marco negro
        monitor.AddPart(CrearCuboide(3.2f, 2.2f, 0.3f, new Vector3(0.2f, 0.2f, 0.8f), 0, 0, -0.25f)); // pantalla azul
        monitor.AddPart(CrearCuboide(0.3f, 1f, 0.3f, new Vector3(0.2f, 0.2f, 0.2f), 0, -1.5f, 0)); // pie gris oscuro
        monitor.AddPart(CrearCuboide(1.5f, 0.2f, 0.8f, new Vector3(0.2f, 0.2f, 0.2f), 0, -2f, 0)); // base gris oscuro
        monitor.SetCenter(x, y, z);
        return monitor;
    }

    private Objeto CrearTeclado(float x, float y, float z)
    {
        Objeto teclado = new Objeto();
        teclado.AddPart(CrearCuboide(4f, 0.2f, 1.5f, new Vector3(0.1f, 0.1f, 0.1f))); // base negro
        for (int i = -1; i <= 2; i++)
            for (int j = -1; j <= 1; j++)
                teclado.AddPart(CrearCuboide(0.6f, 0.1f, 0.5f, new Vector3(1f, 1f, 1f), i * 0.7f, 0.15f, j * 0.6f)); // teclas blancas
        teclado.SetCenter(x, y, z);
        return teclado;
    }

    private Part CrearCuboide(float width, float height, float depth, Vector3 color, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
    {
        Part parte = new Part(color);
        float w = width / 2f;
        float h = height / 2f;
        float d = depth / 2f;

        Vertex[] vertices = new Vertex[]
        {
            new Vertex(-w + offsetX,-h + offsetY,-d + offsetZ),
            new Vertex( w + offsetX,-h + offsetY,-d + offsetZ),
            new Vertex( w + offsetX, h + offsetY,-d + offsetZ),
            new Vertex(-w + offsetX, h + offsetY,-d + offsetZ),
            new Vertex(-w + offsetX,-h + offsetY, d + offsetZ),
            new Vertex( w + offsetX,-h + offsetY, d + offsetZ),
            new Vertex( w + offsetX, h + offsetY, d + offsetZ),
            new Vertex(-w + offsetX, h + offsetY, d + offsetZ)
        };

        int[,] carasIdx = new int[,]
        {
            {0,1,2,3},{4,5,6,7},{0,1,5,4},
            {2,3,7,6},{0,3,7,4},{1,2,6,5}
        };

        for (int i = 0; i < 6; i++)
        {
            Face face = new Face();
            for (int j = 0; j < 4; j++)
                face.AddVertex(vertices[carasIdx[i, j]]);
            parte.AddFace(face);
        }

        return parte;
    }
}