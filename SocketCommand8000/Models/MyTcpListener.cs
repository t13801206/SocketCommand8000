using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketCommand8000.Models
{
    public class MyTcpListener
    {
        private readonly string _address;
        private readonly int _port;
        private readonly TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private const int BUFFER_SIZE = 128;

        public MyTcpListener(string address, int port)
        {
            _address = address;
            _port = port;

            _listener = new TcpListener(IPAddress.Parse(address), port);
        }

        public void Start()
        {
            Debug.WriteLine($"start");
            _listener.Start();

            var token = _cancellationTokenSource.Token;
            Debug.WriteLine(token.CanBeCanceled);
            var listenTask = Task.Run(() => Listen(token), token);
        }

        public void Stop()
        {
            Debug.WriteLine($"stop");
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }

        private void Listen(CancellationToken cancellationToken)
        //private void Listen()
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine($"{nameof(Listen)} received CancellationRequest");
                        break;
                    }

                    if (_listener.Pending())
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        Debug.WriteLine($"{_listener.LocalEndpoint} accept {client.Client.RemoteEndPoint}");
                        try
                        {
                            byte[] message = Common.Receive(client);
                            Common.Print($"{_listener.LocalEndpoint} received from {client.Client.LocalEndPoint}", message);

                            SendBackOK(client);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"listen error {e.Message}");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine($"{e.Message}");
            }
        }

        private void SendBackOK(TcpClient client)
        {
            byte[] message = new byte[] { 0x04, 0x41, 0x42, 0x43 };

            try
            {
                var stream = client.GetStream();
                stream.Write(message, 0, message.Length);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}");
            }
        }
    }
}
