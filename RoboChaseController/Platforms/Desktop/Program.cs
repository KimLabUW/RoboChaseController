using RoboChaseController.Tasks;
using Uno.UI.Hosting;

namespace RoboChaseController;
internal class Program
{
    //[STAThread]
    public static void Main(string[] args)
    {
        var controller = new MainController(); // move this somewhere withing the GUI? And/or otherwise setup communication between the main controller and the GUI
        controller.Run();

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
