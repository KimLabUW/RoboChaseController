using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace RoboChaseController.Tasks;
public class ImageCapture : Processor
{
    public ImageCapture(Config config, ChannelReader<ImageData>? channelReader = null, ChannelWriter<ImageData>? channelWriter = null) : base(config, channelReader, channelWriter)
    {

    }

    internal override void OnMessageRecieved(ImageData imageData)
    {

    }
}
