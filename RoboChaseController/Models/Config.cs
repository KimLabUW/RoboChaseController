using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenCvSharp.Flann;

namespace RoboChaseController.Models;
public class Config
{
    private bool Updates { get; set; } = false;
    public double RatHysteresis { get; private set; } = 0.9;
    public double RobotHysteresis { get; private set; } = 0.9;

    public Config()
    {
        // TODO: read the config file
    }

    public void Update(double? ratHysteresis = null, double? robotHysteresis = null, bool save = true)
    {
        // Update the Parameters
        if (ratHysteresis != null)
        {
            RatHysteresis = ratHysteresis.Value;
            Updates = true;
        }
        if (robotHysteresis != null)
        {
            RobotHysteresis = robotHysteresis.Value;
            Updates = true;
        }
        // TODO: add more parameter updates here

        // Save the Updated Config File
        if (save) 
        { 
            Save(); 
        }
    }

    public void Save()
    {
        if (Updates)
        {
            // TODO: save the updated config file
            Updates = false;
        }
    }
}
