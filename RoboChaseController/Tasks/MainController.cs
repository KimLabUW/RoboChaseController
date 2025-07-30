using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class MainController : IDisposable
{
    public Config Config { get; }
    public ImageCapture ImageCapture { get; private set; }
    public ImageProcessor ImageProcessor { get; private set; }
    public TrackingModel TrackingModel { get; private set; }
    public RobotController RobotController { get; private set; }

    public MainController(Config config)
    {
        Config = config;
        //Channel<ImageData, ImageData> IC2IP = new Channel<ImageData, ImageData>().;
        //ImageCapture = new ImageCapture(Config, , )
    }

    public void Dispose()
    {
        // TODO: update this
    }
}
