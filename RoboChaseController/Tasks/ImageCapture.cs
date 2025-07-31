using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace RoboChaseController.Tasks;
public class ImageCapture : Processor
{
    public ImageCapture() : base()
    {

    }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        // this will likely stay empty
    }
}
