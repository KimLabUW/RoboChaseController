using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class LEDImageProcessor : ImageProcessor
{
    public LEDImageProcessor(Config config, ChannelReader<ImageData>? channelReader = null, ChannelWriter<ImageData>? channelWriter = null) : base(config, channelReader, channelWriter) { }

    internal override void CalculatePositionsAndOrientations(ImageData imageData)
    {
        throw new NotImplementedException(); // TODO: find the LEDs, do elliptical fits, and write the position/orientation data to the imageData object (e.g. imageData.Rat.X, imageData.Robot.theta)
    }
}
