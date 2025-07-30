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
    internal Config Config { get; set; }
    public bool Listening { get; private set; } = false;
    private ChannelReader<ImageData>? ChannelReader { get; set; } = null;
    private ChannelWriter<ImageData>? ChannelWriter { get; set; } = null;
    private CancellationTokenSource? ListenerCancellationTokenSource { get; set; } = null;
    private object _lock { get; } = new object();

    public Processor(Config config, ChannelReader<ImageData>? channelReader = null, ChannelWriter<ImageData>? channelWriter = null)
    {
        Config = config;
        UpdateConfig(config);
        UpdateChannels(channelReader, channelWriter);
        Start();
    }

    public virtual void UpdateConfig(Config config) { }

    public void UpdateChannels(ChannelReader<ImageData>? channelReader = null, ChannelWriter<ImageData>? channelWriter = null)
    {
        if (Listening)
        {
            throw new InvalidOperationException("channel must be closed before it can be updated (Process.Stop())");
        }
        ChannelReader = channelReader;
        ChannelWriter = channelWriter;
    }

    public void Start()
    {
        if (ChannelReader != null)
        {
            Listen();
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
                    Task.Factory.StartNew(() => RecieveMessages(), ListenerCancellationTokenSource.Token);
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
