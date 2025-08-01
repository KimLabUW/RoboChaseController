using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Uno.Disposables;

namespace RoboChaseController.Tasks;
public abstract class Processor<TWrite, TRead> : IDisposable
{
    public bool Listening { get; private set; } = false;
    private ChannelReader<TRead>? ChannelReader { get; set; } = null;
    private ChannelWriter<TWrite>? ChannelWriter { get; set; } = null;
    private CancellationTokenSource? ListenerCancellationTokenSource { get; set; } = null;
    private object _lock { get; } = new object();

    public Processor()
    {
        
    }

    protected Channel<TWrite> MakeChannel()
    {
        return Channel.CreateUnbounded<TWrite>();
    }

    public void AddChannel(Processor<TWrite> reciever)
    {
        Channel<TWrite> channel = MakeChannel();
        ChannelWriter = channel.Writer;
        reciever.ChannelReader = channel.Reader;
    }

    public void AddChannel(out ChannelWriter<TWrite> channelWriter, Processor<TWrite> reciever)
    {
        Channel<TWrite> channel = MakeChannel();
        channelWriter = channel.Writer;
        reciever.ChannelReader = channel.Reader;
    }

    public void AddChannel(out ChannelReader<TWrite> channelReader)
    {
        Channel<TWrite> channel = MakeChannel();
        ChannelWriter = channel.Writer;
        channelReader = channel.Reader;
    }

    public void AddChannel(out ChannelWriter<TWrite> channelWriter, out ChannelReader<TWrite> channelReader)
    {
        Channel<TWrite> channel = MakeChannel();
        channelWriter = channel.Writer;
        channelReader = channel.Reader;
    }

    public void AddChannelReader(ChannelReader<TRead> channelReader)
    {
        if (Listening)
        {
            throw new InvalidOperationException("channel must be closed before it can be updated (Process.Stop())");
        }
        ChannelReader = channelReader;
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

    public virtual void NewRecording() { }

    public virtual void Start()
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

    public virtual void Stop()
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

    public void SendMessage(TWrite messageData)
    {
        if (ChannelWriter != null)
        {
            ChannelWriter.WriteAsync(messageData);
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
                    TRead messageData = await ChannelReader.ReadAsync();
                    if (messageData != null)
                    {
                        OnMessageRecieved(messageData);
                    }
                }
            }
        }
    }

    internal abstract void OnMessageRecieved(TRead messageData);

    public void Dispose()
    {
        Stop();
        ChannelReader?.TryDispose();
        ChannelWriter?.TryDispose();
        ListenerCancellationTokenSource?.Dispose();
    }
}

public abstract class Processor<T> : Processor<T, T> { }

public abstract class Processor : Processor<ImageData> { }

public class OutgoingChannel<T> : Processor<T>
{
    public OutgoingChannel(out ChannelReader<T> channelReader)
    {
        AddChannel(out channelReader);
    }

    internal override void OnMessageRecieved(T messageData)
    {
        SendMessage(messageData); // Parrot the data - ideally this should not be used
    }
}

