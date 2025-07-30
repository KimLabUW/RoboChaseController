using Uno.UI.Hosting;

namespace RoboChaseController;
internal class Program
{
    //[STAThread]
    public static void Main(string[] args)
    {

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();
        
        host.Run(); // THIS IS BLOCKING (even when using RunAsync() instead)
    }
}
