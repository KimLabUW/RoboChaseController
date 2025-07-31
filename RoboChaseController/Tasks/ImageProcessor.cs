using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public abstract class ImageProcessor : Processor
{
    public ImageProcessor() : base() { }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        CalculatePositionsAndOrientations(imageData);
        SendMessage(imageData);
    }

    internal abstract void CalculatePositionsAndOrientations(ImageData imageData);
}
