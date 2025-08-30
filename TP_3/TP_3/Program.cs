using OpenTK.Windowing.Desktop;

class Program
{
    static void Main()
    {
        var game = new TP3Game(GameWindowSettings.Default, NativeWindowSettings.Default);
        game.Run();
    }
}