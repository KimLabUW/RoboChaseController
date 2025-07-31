using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class RobotController : Processor
{
    public RobotController() : base()
    {

    }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        throw new NotImplementedException();
    }
}
