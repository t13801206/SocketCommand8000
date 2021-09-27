using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
        private Task _listenTask;

        public MyTcpListener(string address, int port)
        {
            _address = address;
            _port = port;
            _listener = new TcpListener(IPAddress.Parse(address), port);
        }

        public void Start()
        {
            if(_cancellationTokenSource.Token.IsCancellationRequested)
            {
                ResetCancelToken();
            }

            _listener.Start();

            var token = _cancellationTokenSource.Token;
            _listenTask = Task.Run(() => Listen(token), token);
            Debug.WriteLine($"start");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
            Debug.WriteLine($"stop");
        }

        public void ResetCancelToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Debug.WriteLine("reset");
        }

        private void Listen(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"loop {_listenTask.Status}");
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
                            byte[] message = Util.Receive(client);
                            Util.Print($"{_listener.LocalEndpoint} received from {client.Client.LocalEndPoint} <<<=== ", message);

                            SendBackOK(client);
                        }
                        catch(System.IO.IOException e)
                        {
                            Debug.WriteLine($"IO listen error {e.Message}");
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"listen error {e.Message}");
                        }
                        finally
                        {
                            client.Close();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine($"{e.Message}");
            }
            finally
            {
                Debug.WriteLine($"loop end {_listenTask.Status}");
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
