using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace RoboChaseController.Tasks;
public class ImageCapture : Processor<ImageData, ImageProcessor>
{
    public ImageCapture() : base()
    {
        // TODO: test camera connection?
    }

    public override void Start()
    {
        base.Start();
        // TODO: start getting frames from the camera
    }

    internal override void OnMessageRecieved(ImageProcessor imageProcessor)
    {
        Stop();
        RemoveChannelWriter();
        AddChannel(imageProcessor);
        Start();
    }
}
