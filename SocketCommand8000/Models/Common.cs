using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace SocketCommand8000.Models
{
    internal class Common
    {
        private const byte BUFFER_SIZE = 128;

        internal static byte[] Receive(TcpClient client)
        {
            byte[] buf = new byte[BUFFER_SIZE];
            int totalReadBytes = 0;
            int bytesToRead = -1;

            var stream = client.GetStream();

            while (true)
            {
                if (totalReadBytes == bytesToRead)
                    break;

                int readBytes = stream.Read(buf, totalReadBytes, BUFFER_SIZE - totalReadBytes);
                bytesToRead = buf[0];

                totalReadBytes += readBytes;
                Debug.WriteLine($"readBytes:{readBytes}, total: {totalReadBytes}");
            }

            Debug.WriteLine("End read");

            byte[] receivedBytes = new byte[totalReadBytes];
            Array.Copy(buf, receivedBytes, totalReadBytes);

            return receivedBytes;
        }

        internal static void Print(string pre, byte[] message)
        {
            Debug.Write(pre);
            for (int i = 0; i < message.Length; i++)
            {
                Debug.Write($"{message[i]}, ");
            }
            Debug.Write("\n");
        }
    }
}
