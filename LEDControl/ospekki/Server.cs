using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Microsoft.Extensions.Logging;

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

        private readonly ILogger<Server> _logger;

        public Server(ILogger<Server> logger = null)
        {
            udpClient = new UdpClient(listenPort);
            _logger = logger;
        }

        public void Run()
        {
            isRunning = true;

            while (isRunning)
            {
                try
                {
                    //192.168.1.196 -> desktop
                    //192.168.1.234 -> pi
                    udpClient.Connect("192.168.1.196", listenPort);

                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.196"), 4445);

                    // Blocks until a message returns on this socket from a remote host.
                    byte[] receivedData = udpClient.Receive(ref RemoteIpEndPoint);

                    receivedData.CopyTo(buffer, 0);

                    Visualizer.GetData(buffer);

                    for(int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    isRunning = false;
                }
            }

            udpClient.Close();
        }

        public void Start()
        {
            serverThread = new Thread(Run);
            serverThread.Start();
        }

        public void StopServer()
        {
            isRunning = false;
        }
    }
}
