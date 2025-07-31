using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class DLCImageProcessor : ImageProcessor
{
    public DLCImageProcessor() : base() { }

    internal override void CalculatePositionsAndOrientations(ImageData imageData)
    {
        // TODO: find the positions/orientations using DeepLabCut, and write the data to the imageData object (e.g. imageData.Rat.X, imageData.Robot.theta)
        // Alex, this is entirely on you, imageData.Frame is where you'll find the video frame to analyze
        throw new NotImplementedException();
    }
}
