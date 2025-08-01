using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RoboChaseController.Models;
public static class Config
{
    private const string ConfigXmlPath = "..\\..\\..\\config.xml";
    private static bool Updates { get; set; } = false;
    private static ConfigModel ConfigModel { get; set; } = new ConfigModel(); // this gets overwritten later
    public static double RatHysteresis { get => ConfigModel.RatHysteresis; set { Updates |= RatHysteresis != value; ConfigModel.RatHysteresis = value; Save(); } }
    public static double RobotHysteresis { get => ConfigModel.RobotHysteresis; set { Updates |= RobotHysteresis != value; ConfigModel.RobotHysteresis = value; Save(); } }
    public static ImageProcessingAlgorithms ImageProcessingAlgorithm { get => ConfigModel.ImageProcessingAlgorithm; set { Updates |= ImageProcessingAlgorithm != value; ConfigModel.ImageProcessingAlgorithm = value; Save(); } }
    public static ChaseAlgorithms ChaseAlgorithm { get => ConfigModel.ChaseAlgorithm; set { Updates |= ChaseAlgorithm != value; ConfigModel.ChaseAlgorithm = value; Save(); } }
    public static bool RatEnabled { get => ConfigModel.RatEnabled; set { Updates |= RatEnabled != value; ConfigModel.RatEnabled = value; Save(); } }
    public static bool RobotEnabled { get => ConfigModel.RobotEnabled; set { Updates |= RobotEnabled != value; ConfigModel.RobotEnabled = value; Save(); } }
    public static bool ShockerEnabled { get => ConfigModel.ShockerEnabled; set { Updates |= ShockerEnabled != value; ConfigModel.ShockerEnabled = value; Save(); } }

    public static void Load()
    {
        object? deserialized;
        using (XmlReader reader = XmlReader.Create(ConfigXmlPath))
        {
            deserialized = new XmlSerializer(typeof(ConfigModel)).Deserialize(reader);
        }
        if (deserialized == null)
        {
            // TODO: log a warning
            ConfigModel = new ConfigModel(); // Return a ConfigModel with Hard-Coded Default Values
        }
        else
        {
            ConfigModel = (ConfigModel)deserialized;
        }
    }

    public static void Save()
    {
        if (Updates)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    ", // 4 spaces matches my editor settings, it is not critical that this remains constant
                NewLineOnAttributes = true,
                Encoding = Encoding.UTF8,
                Async = true
            };
            
            using (XmlWriter writer = XmlWriter.Create(ConfigXmlPath, settings))
            {
                new XmlSerializer(typeof(ConfigModel)).Serialize(writer, ConfigModel);
            }
            Updates = false;
        }
    }
}

[XmlRoot(ElementName = "Config")]
public class ConfigModel
{
    [XmlElement]
    public double RatHysteresis { get; set; } = 0.9;
    [XmlElement]
    public double RobotHysteresis { get; set; } = 0.9;
    [XmlElement]
    public ImageProcessingAlgorithms ImageProcessingAlgorithm { get; set; } = ImageProcessingAlgorithms.LED;
    [XmlElement]
    public ChaseAlgorithms ChaseAlgorithm { get; set; } = ChaseAlgorithms.StalkAndLunge;
    [XmlElement]
    public bool RatEnabled { get; set; } = true;
    [XmlElement]
    public bool RobotEnabled { get; set; } = true;
    [XmlElement]
    public bool ShockerEnabled { get; set; } = true;
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
