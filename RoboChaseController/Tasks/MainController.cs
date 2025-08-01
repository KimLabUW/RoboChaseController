using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class MainController : Processor
{
    public ImageCapture ImageCapture { get; private set; }
    public ImageProcessor ImageProcessor { get; private set; }
    public TrackingModel TrackingModel { get; private set; }
    public RobotController RobotController { get; private set; }
    private OutgoingChannel<ImageProcessor> ImageCaptureChannel { get; }

    public MainController() : base()
    {
        // Load the Config File
        Config.Load();

        // Create the Various Tasks
        ImageCapture = new ImageCapture();
        TrackingModel = new TrackingModel();
        RobotController = new RobotController();
        switch (Config.ImageProcessingAlgorithm)
        {
            case ImageProcessingAlgorithms.LED:
                ImageProcessor = new LEDImageProcessor();
                break;
            case ImageProcessingAlgorithms.DeepLabCut:
                ImageProcessor = new DLCImageProcessor();
                break;
            default:
                throw new ArgumentException(String.Format("Unrecognized Image Processing Algorithm Key: {0}", Config.ImageProcessingAlgorithm));
        }

        // Create the Connections Between the Threads
        ImageCapture.AddChannel(ImageProcessor);
        ImageProcessor.AddChannel(TrackingModel);
        TrackingModel.AddChannel(this);
        AddChannel(RobotController);

        // Add a Connection to the Image Capture Processor from the Main Controller
        ImageCaptureChannel = new OutgoingChannel<ImageProcessor>(out ChannelReader<ImageProcessor> channelReader);
        ImageCapture.AddChannelReader(channelReader);
    }

    public override void NewRecording()
    {
        ImageCapture.NewRecording();
        ImageProcessor.NewRecording();
        TrackingModel.NewRecording();
        RobotController.NewRecording();
    }

    public void Run()
    {
        // Start the Threads in Reverse Order (helps ensure that listeners are listening to the channels before messages are being sent)
        RobotController.Start();
        Start();
        TrackingModel.Start();
        ImageProcessor.Start();
        ImageCapture.Start();
    }

    public override void Start()
    {
        base.Start(); // begins listening to messages from the Tracking Model
        // TODO: begin listening for messages from the GUI
    }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        // TODO: communicate with GUI to Add User-Inputs to the Image Data
        SendMessage(imageData);
    }
}
