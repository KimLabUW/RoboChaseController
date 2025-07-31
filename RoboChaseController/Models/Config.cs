using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenCvSharp.Flann;

namespace RoboChaseController.Models;
public static class Config
{
    private static bool Updates { get; set; } = false;
    public static double RatHysteresis { get; private set; } = 0.9;
    public static double RobotHysteresis { get; private set; } = 0.9;
    public static ImageProcessingAlgorithms ImageProcessingAlgorithm { get; private set; } = ImageProcessingAlgorithms.LED;
    public static ChaseAlgorithms ChaseAlgorithm { get; private set; } = ChaseAlgorithms.StalkAndLunge;

    public static void Load()
    {
        //throw new NotImplementedException(); // TODO: load the xml file
    }

    public static void Update(double? ratHysteresis = null, double? robotHysteresis = null, ImageProcessingAlgorithms? imageProcessingAlgorithm = null, ChaseAlgorithms? chaseAlgorithm = null, bool save = true)
    {
        // Update the Parameters
        if (ratHysteresis != null)
        {
            Updates |= RatHysteresis != ratHysteresis.Value;
            RatHysteresis = ratHysteresis.Value;
        }
        if (robotHysteresis != null)
        {
            Updates |= RobotHysteresis != robotHysteresis.Value;
            RobotHysteresis = robotHysteresis.Value;
        }
        if (imageProcessingAlgorithm != null)
        {
            Updates |= ImageProcessingAlgorithm != imageProcessingAlgorithm.Value;
            ImageProcessingAlgorithm = imageProcessingAlgorithm.Value;
        }
        if (chaseAlgorithm != null)
        {
            Updates |= ChaseAlgorithm != chaseAlgorithm.Value;
            ChaseAlgorithm = chaseAlgorithm.Value;
        }
        // TODO: add more parameter updates here

        // Save the Updated Config File
        if (save) 
        { 
            Save(); 
        }
    }

    public static void Save()
    {
        if (Updates)
        {
            // TODO: save the updated config file
            Updates = false;
        }
    }
}

public enum ImageProcessingAlgorithms
{
    LED,
    DeepLabCut
}

public enum ChaseAlgorithms
{
    StalkAndLunge,
    Stalk,
    Watch
}
