using UnityEngine;
using System.IO;
using System.Collections.Generic;
using uOSC;

public class uOscClientTDP : MonoBehaviour
{
    private const int BufferSize = 8192;
    private const int MaxQueueSize = 100;

    public string address = "127.0.0.1";
    public int port = 39539;

#if NETFX_CORE
    Udp udp_ = new Uwp.Udp();
    Thread thread_ = new Uwp.Thread();
#else
    Udp udp_ = new uOSC.DotNet.Udp();
    Thread thread_ = new uOSC.DotNet.Thread();
#endif
    Queue<object> messages_ = new Queue<object>();
    object lockObject_ = new object();

    void OnEnable()
    {
        StartThread();
    }

    void OnDisable()
    {
        StopThread();
    }
    public void StartThread()
    {
        udp_.StartClient(address, port);
        thread_.Start(UpdateSend);
    }

    public void StopThread()
    {
        thread_.Stop();
        udp_.Stop();
    }

    void UpdateSend()
    {
        while (messages_.Count > 0)
        {
            object message;
            lock (lockObject_)
            {
                message = messages_.Dequeue();
            }

            using (var stream = new MemoryStream(BufferSize))
            {
                if (message is Message)
                {
                    ((Message)message).Write(stream);
                }
                else if (message is Bundle)
                {
                    ((Bundle)message).Write(stream);
                }
                else
                {
                    return;
                }
                udp_.Send(Util.GetBuffer(stream), (int)stream.Position);
            }
        }
    }

    void Add(object data)
    {
        lock (lockObject_)
        {
            messages_.Enqueue(data);

            while (messages_.Count > MaxQueueSize)
            {
                messages_.Dequeue();
            }
        }
    }

    public void Send(string address, params object[] values)
    {
        Send(new Message()
        {
            address = address,
            values = values
        });
    }

    public void Send(Message message)
    {
        Add(message);
    }

    public void Send(Bundle bundle)
    {
        Add(bundle);
    }
}
