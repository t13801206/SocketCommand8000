using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace SocketCommand8000.Models
{
    public class MyTcpClient
    {
        private readonly string _address;
        private readonly int _port;
        //private TcpClient _client;

        public MyTcpClient(string address, int port)
        {
            _address = address;
            _port = port;
        }

        private TcpClient CreateTcpClinet()
        {
            Debug.WriteLine("create client");
            return new TcpClient()
            {
                SendTimeout = 3000,
                ReceiveTimeout = 3000,
            };
        }

        public void Connect(TcpClient client)
        {
            if (client.Connected)
            {
                Debug.WriteLine($"{client.Client.LocalEndPoint} already connected to {client.Client.RemoteEndPoint}");
                return;
            }

            try
            {
                client.Connect(_address, _port);
                Debug.WriteLine($"{client.Client.LocalEndPoint} connect succeeded ===>>> {client.Client.RemoteEndPoint}");
            }
            catch (Exception e)
            {
                CreateTcpClinet();
                Debug.WriteLine($"{e.Message}");
            }
        }

        public void Send(byte[] message)
        {
            var client = CreateTcpClinet();
            Connect(client);

            try
            {
                Common.Print($"{client.Client.LocalEndPoint} -> {client.Client.RemoteEndPoint} Sending Message ===>>> ", message);

                using NetworkStream stream = client.GetStream();
                stream.Write(message, 0, message.Length);
                
                byte[] receiveMessage = Common.Receive(client);
                Common.Print($"{client.Client.LocalEndPoint} <- {client.Client.RemoteEndPoint} Received Message: <<<===", receiveMessage);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"client send error !! {e.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
