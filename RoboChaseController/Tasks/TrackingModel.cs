using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Graphics.DirectX;

namespace RoboChaseController.Tasks;
public class TrackingModel : Processor
{
    private ImageData? PreviousImageData { get; set; } = null;

    public TrackingModel() : base() { }

    public override void NewRecording()
    {
        PreviousImageData = null;
    }

    internal override void OnMessageRecieved(ImageData imageData)
    {
        // Check that Previous Data Exists (otherwise we obviously can't compute any sort of velocity )
        if (PreviousImageData != null)
        {
            // Check that the Frame is New
            double dt = imageData.TimeStamp.Subtract(PreviousImageData.TimeStamp).TotalMicroseconds;
            if (dt <= 0) { return; } // the frame is old and was recieved out of order, do not compute velocities, do not update the GUI, do not send commands to the robot based on it

            // Compute the Velocities
            if (imageData.HasRat)
            {
                GetVelocity(PreviousImageData.Rat, imageData.Rat, Config.RatHysteresis, dt);
            }
            if (imageData.HasRobot)
            {
                GetVelocity(PreviousImageData.Robot, imageData.Robot, Config.RobotHysteresis, dt);
            }
        }

        // Store the New Frame
        PreviousImageData = imageData;

        // Send the Updated Image Data to the Main Controller/GUI (discard the created Task)
        SendMessage(imageData);
    }

    private static void GetVelocity(TrackedObject previous, TrackedObject current, double hysteresis, double dt)
    {
        // hysteresis model : V_t ~= h*V_{t-1} + (1 - h)*DX/DT : should reduce noise, which would otherwise be present/amplified by the numeric differentiation
        hysteresis = previous.HasVelocity ? hysteresis : 0;
        double dx = current.X - previous.X;
        double dy = current.Y - previous.Y;
        double vx = dx / dt;
        double vy = dy / dt;
        if (previous.HasVelocity) // can't use hysteresis if no previous velocity is available
        {
            vx = hysteresis * previous.VX + (1 - hysteresis) * vx;
            vy = hysteresis * previous.VY + (1 - hysteresis) * vy;
        }
        current.VX = vx;
        current.VY = vy;
    }


}
