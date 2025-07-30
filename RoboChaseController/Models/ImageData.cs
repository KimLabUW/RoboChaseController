using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace RoboChaseController.Models;
/// <summary>
/// A Class for Storing All Data About Each Video Frame from the Overhead Camera (including the positions/orientations/velocities of both the rat and the robot)
/// </summary>
public class ImageData
{
    /*---------------PRIVATE GLOBAL ATTRIBUTES---------------*/
    private static long Id_global { get; set; } = 0;

    /*---------------PUBLIC GLOBAL ATTRIBUTES---------------*/
    public static bool HasRat { get; set; } = true; // TODO: move this to the config
    public static bool HasRobot { get; set; } = true; // TODO: move this to the config
    public static bool HasShocker { get; set; } = true; // TODO: move this to the config

    /*---------------DATA ATTRIBUTES---------------*/
    public long Id { get; }
    public DateTime TimeStamp { get; }
    public double Seconds { get; }
    public Mat Frame { get; }
    public TrackedObject Rat { get; }
    public TrackedObject Robot { get; }

    /*---------------COMPUTED ATTRIBUTES---------------*/
    public bool HasAllData => (!HasRat || Rat.HasAllData) && (!HasRobot || Robot.HasAllData); // for safety, this should be true of all frames which are used to guide the robot

    /*---------------METHODS---------------*/
    public ImageData(Mat frame)
    {
        Id = Id_global++;
        TimeStamp = DateTime.Now;
        Seconds = TimeStamp.Subtract(DateTime.MinValue).TotalSeconds;
        Frame = frame;
        Rat = new TrackedObject();
        Robot = new TrackedObject() { Phi = 0 }; // robot is always assumed to be flat?
    }
}

/// <summary>
/// A Class for Tracking the 2D Position/Orientation/Velocity of an Object in the Environment (all attributes can only be set once outside of the OverrideValue method)
/// </summary>
public class TrackedObject
{
    /*---------------DATA ATTRIBUTES---------------*/
    private double? _x { get; set; } = null;
    /// <value>the object's x-coordinate in pixels.</value>
    public double X { get { return _x ?? -1; } set { _x ??= value; } }
    private double? _y { get; set; } = null;
    /// <value>the object's y-coordinate in pixels.</value>
    public double Y { get { return _y ?? -1; } set { _y ??= value; } }
    private double? _theta { get; set; } = null;
    /// <value>the object's orientation in the x-y plane in radians.</value>
    public double Theta { get { return _theta ?? -1; } set { _theta ??= value; } }
    private double? _phi { get; set; } = null;
    /// <value>the object's angle off of the x-y plane in radians.</value>
    public double Phi { get { return _phi ?? -1; } set { _phi ??= value; } }
    private double? _vx { get; set; } = null;
    /// <value>the object's x-velocity in pixels/second.</value>
    public double VX { get { return _vx ?? -1; } set { _vx ??= value; } }
    private double? _vy { get; set; } = null;
    /// <value>the object's y-velocity in pixels/second.</value>
    public double VY { get { return _vy ?? -1; } set { _vy ??= value; } }

    /*---------------COMPUTED ATTRIBUTES---------------*/
    /// <value>checks that (x, y) position, (vx, vy) velocity, and (theta) orientation information is all present</value>
    public bool HasAllData => new List<double?>([_x, _y, _theta, _vx, _vy ]).All(x => x != null);

    /*---------------METHODS---------------*/
    /// <summary>
    /// Compute the Speed of a Tracked Object (pythagorean of its velocity components)
    /// </summary>
    public double Speed()
    {
        return Math.Sqrt(Math.Pow(VX, 2) + Math.Pow(VY, 2));
    }

    /// <summary>
    /// Computes the Euclidean Distance Between 2 Tracked Objects
    /// </summary>
    public double Distance(TrackedObject other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
    }

    /// <summary>
    /// Computes the Direction Vector Between 2 Objects (Returns an Array of [DX, DY])
    /// </summary>
    public double[] Direction(TrackedObject other)
    {
        return [X - other.X, Y - other.Y];
    }

    /// <summary>
    /// Override Any of the Stored Values (this is normally disallowed, and is not recommended - use with caution)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <param name="dz"></param>
    /// <param name="vx"></param>
    /// <param name="vy"></param>
    public void OverrideValue(double? x = null, double? y = null, double? theta = null, double? phi = null, double? vx = null, double? vy = null)
    {
        _x = x ?? _x;
        _y = y ?? _y;
        _theta = theta ?? _theta;
        _phi = phi ?? _phi;
        _vx = vx ?? _vx;
        _vy = vy ?? _vy;
    }
}
