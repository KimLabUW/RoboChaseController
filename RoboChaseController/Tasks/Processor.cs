using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Uno.Disposables;

namespace RoboChaseController.Tasks;
public abstract class Processor : IDisposable
{
    public bool Listening { get; private set; } = false;
    private ChannelReader<ImageData>? ChannelReader { get; set; } = null;
    private ChannelWriter<ImageData>? ChannelWriter { get; set; } = null;
    private CancellationTokenSource? ListenerCancellationTokenSource { get; set; } = null;
    private object _lock { get; } = new object();

    public Processor()
    {
        
    }

    public void AddChannel(Processor reciever)
    {
        Channel<ImageData> channel = Channel.CreateUnbounded<ImageData>();
        ChannelWriter = channel.Writer;
        reciever.ChannelReader = channel.Reader;
    }

    public void RemoveChannelReader()
    {
        if (Listening)
        {
            throw new InvalidOperationException("channel must be closed before it can be updated (Process.Stop())");
        }
        ChannelReader?.TryDispose();
        ChannelReader = null;
    }

    public void RemoveChannelWriter()
    {
        ChannelWriter?.TryDispose();
        ChannelWriter = null;
    }

    public void RemoveChannels(bool removeReader = true, bool removeWriter = true)
    {
        if (removeReader)
        {
            RemoveChannelReader();
        }
        if (removeWriter)
        {
            RemoveChannelWriter();
        }
    }

    public void Start()
    {
        if (ChannelReader != null)
        {
            Listen();
        }
        else
        {
            throw new ArgumentNullException(nameof(ChannelReader));
        }
    }

    public void Listen()
    {
        if (!Listening)
        {
            lock(_lock)
            {
                if (!Listening)
                {
                    Listening = true;
                    ListenerCancellationTokenSource = new CancellationTokenSource();
                    Task.Factory.StartNew(RecieveMessages, ListenerCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
        }
    }

    public void Stop()
    {
        if (Listening)
        {
            lock(_lock)
            {
                if (Listening)
                {
                    Listening = false;
                    ListenerCancellationTokenSource?.Cancel();
                }
            }
        }
    }

    public void SendMessage(ImageData imageData)
    {
        if (ChannelWriter != null)
        {
            ChannelWriter.WriteAsync(imageData);
        }
    }

    private async Task RecieveMessages()
    {
        if (ChannelReader != null)
        {
            while (Listening)
            {
                if (await ChannelReader.WaitToReadAsync())
                {
                    ImageData imageData = await ChannelReader.ReadAsync();
                    if (imageData != null)
                    {
                        OnMessageRecieved(imageData);
                    }
                }
            }
        }
    }

    internal abstract void OnMessageRecieved(ImageData imageData);

    public void Dispose()
    {
        Stop();
        ChannelReader?.TryDispose();
        ChannelWriter?.TryDispose();
        ListenerCancellationTokenSource?.Dispose();
    }
}
