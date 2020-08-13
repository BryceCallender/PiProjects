using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LEDControl.ospekki
{
    //This is not my own code it is a C# port of this instead of a Java server: 
    //https://github.com/ospekki/music-visualizer-server/blob/master/src/visualizer/Server.java
    public class Server
    {
        private Thread serverThread;

        private UdpClient udpClient;
        private byte[] buffer = new byte[256];
        private bool isRunning;

        private readonly int listenPort = 4445;
        private readonly string hostname = "192.168.1.196";

        public Server()
        {
            udpClient = new UdpClient(listenPort);
        }

        public void Run()
        {
            isRunning = true;

            //192.168.1.196 -> desktop
            //192.168.1.234 -> pi
            //udpClient.Connect(hostname, listenPort);

            try
            {
                while (isRunning)
                {
                
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(hostname), listenPort);

                    // Blocks until a message returns on this socket from a remote host.
                    byte[] receivedData = udpClient.Receive(ref RemoteIpEndPoint);

                    Debug.WriteLine(receivedData.Length);

                    receivedData.CopyTo(buffer, 0);

                    Visualizer.GetData(buffer);

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                isRunning = false;
            }
            finally
            {
                udpClient.Close();
            }
        }

        public void Start()
        {
            Debug.WriteLine("Starting server...");

            serverThread = new Thread(Run);
            serverThread.Start();
        }

        public void StopServer()
        {
            Debug.WriteLine("Stopping server...");
            isRunning = false;
        }
    }
}
