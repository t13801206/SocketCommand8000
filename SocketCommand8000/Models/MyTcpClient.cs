using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SocketCommand8000.Models
{
    public class MyTcpClient
    {
        private readonly string _address;
        private readonly int _port;
        private readonly IPEndPoint _localEP;
        private MyClient _client;

        public MyTcpClient(string address, int port, string localAddress = "127.0.0.1", int localPort = 9000)
        {
            _address = address;
            _port = port;
            _localEP = new IPEndPoint(IPAddress.Parse(localAddress), localPort);
            _client = CreateTcpClinet();
        }

        private MyClient CreateTcpClinet()
        {
            var client = new MyClient(_localEP)
            {
                SendTimeout = 3000,
                ReceiveTimeout = 3000,
            };
            Debug.WriteLine($"create client {client.Client.LocalEndPoint}");
            return client;
        }

        public void Connect()
        {
            if (_client.Connected)
            {
                Debug.WriteLine($"{_client.Client.LocalEndPoint} already connected to {_client.Client.RemoteEndPoint}");
                return;
            }

            try
            {
                if (_client.IsDead)
                    _client = CreateTcpClinet();

                _client.Connect(_address, _port);
                Debug.WriteLine($"{_client.Client.LocalEndPoint} connect succeeded ===>>> {_client.Client.RemoteEndPoint}");
            }
            catch(SocketException e)
            {
                Debug.WriteLine($"re-create client {e.Message}");
                _client.Close();
                _client = CreateTcpClinet();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}");
            }
        }

        public void Send(byte[] message)
        {
            try
            {
                if(!_client.Connected)
                {
                    Debug.WriteLine("not connected");
                    return;
                }
                Util.Print($"{_client.Client.LocalEndPoint} -> {_client.Client.RemoteEndPoint} Sending Message ===>>> ", message);

                using NetworkStream stream = _client.GetStream();
                stream.Write(message, 0, message.Length);
                
                byte[] receiveMessage = Util.Receive(_client);
                Util.Print($"{_client.Client.LocalEndPoint} <- {_client.Client.RemoteEndPoint} Received Message: <<<===", receiveMessage);

                _client.Close();
            }
            catch(System.IO.IOException e)
            {
                Debug.WriteLine($"io exception {e.Message}");

            }
            catch (Exception e)
            {
                Debug.WriteLine($"client send error !! {e.Message}");
            }
        }
    }

    internal class MyClient : TcpClient
    {
        public MyClient(IPEndPoint localEP) : base(localEP)
        {
        }

        public bool IsDead { get; internal set; } = false;

        protected override void Dispose(bool disposing)
        {
            IsDead = true;
            base.Dispose(disposing);
        }
    }
}
