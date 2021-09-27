using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace SocketCommand8000.Models
{
    internal class Util
    {
        private const byte BUFFER_SIZE = 128;

        internal static byte[] Receive(TcpClient client)
        {
            byte[] buf = new byte[BUFFER_SIZE];
            int totalReadBytes = 0;
            int bytesToRead = -1;

            var stream = client.GetStream();
            stream.ReadTimeout = 4000;

            while (true)
            {
                if (totalReadBytes == bytesToRead)
                    break;

                int readBytes = stream.Read(buf, totalReadBytes, BUFFER_SIZE - totalReadBytes);
                totalReadBytes += readBytes;
                Debug.WriteLine($"toRead: {bytesToRead}, readBytes:{readBytes}, total: {totalReadBytes}");

                if (readBytes == 0)
                    break; 

                bytesToRead = buf[0];
            }
            Debug.WriteLine($"End read, totalRead: {totalReadBytes} (toRead {bytesToRead})");

            if (totalReadBytes == bytesToRead)
                Debug.WriteLine("ok");
            else
                Debug.WriteLine("ng");
                    
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
