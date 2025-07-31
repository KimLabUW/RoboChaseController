using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoboChaseController.Tasks;
public class TrackingModel : Processor
{
    private ImageData? PreviousImageData { get; set; } = null;
    private bool FirstComputation { get; set; } = true;
    public TrackingModel() : base() { }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        // Check that Previous Data Exists (otherwise we obviously can't compute any sort of velocity )
        if (PreviousImageData != null)
        {
            // Check that the Frame is New
            double dt = imageData.TimeStamp.Subtract(PreviousImageData.TimeStamp).TotalMicroseconds;
            if (dt <= 0) { return; } // the frame is old and was recieved out of order, do not compute velocities, do not update the GUI, do not send commands to the robot based on it

            // Compute the Velocities
            if (ImageData.HasRat)
            {
                GetVelocity(PreviousImageData.Rat, imageData.Rat, FirstComputation ? 0 : Config.RatHysteresis, dt);
            }
            if (ImageData.HasRobot)
            {
                GetVelocity(PreviousImageData.Robot, imageData.Robot, FirstComputation ? 0 : Config.RobotHysteresis, dt);
            }

            // Start Using Hysteresis in the Future
            FirstComputation = false;
        }

        // Store the New Frame
        PreviousImageData = imageData;

        // Send the Updated Image Data to the Main Controller/GUI (discard the created Task)
        SendMessage(imageData);
    }

    private static void GetVelocity(TrackedObject previous, TrackedObject current, double hysteresis, double dt)
    {
        // hysteresis model : V_t ~= h*V_{t-1} + (1 - h)*DX/DT : should reduce noise, which would otherwise be present/amplified by the numeric differentiation
        current.VX = hysteresis * previous.VX + (1 - hysteresis) * (current.X - previous.X) / dt;
        current.VY = hysteresis * previous.VY + (1 - hysteresis) * (current.Y - previous.Y) / dt;
    }
}
