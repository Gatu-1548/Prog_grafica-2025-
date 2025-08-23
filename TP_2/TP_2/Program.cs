using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using TP_2;

namespace EjemploOpenTK
{
    public static class Program
    {
        public static void Main()
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Cubo CPU OpenTK 4.9.4"
            };

            using (var ventana = new Ventana(gws, nws))
            {
                ventana.Run();
            }
        }
    }
}