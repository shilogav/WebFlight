using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlightSimulator.Model
{
    public class MyTcpClient : ITelnetClient
    {
        private TcpClient client;

        public MyTcpClient() {}

        private static bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock)
        {
            try
            {
                Task task = Task.Factory.StartNew(() => codeBlock());
                task.Wait(timeSpan);
                return task.IsCompleted;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }
        }


        public void connect(string ip, int port, int maxTimeInSec = 120)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            client = new TcpClient();
            Thread thread = new Thread(() =>
            {
                bool completed = ExecuteWithTimeLimit(TimeSpan.FromSeconds(maxTimeInSec), () =>
                {
                    bool stop = false;
                    while (!stop)
                    {
                        try
                        {
                            client.Connect(ep);
                            stop = true;
                        }
                        catch (Exception e) { }
                    }
                });
                if (!completed)
                {
                    throw new Exception("Could not connect to ip:" + ip + "port:" + port );
                }
            });
            
            thread.Start();
        }

        public void disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
        }

        public bool isConnected()
        {
            return client != null && client.Connected;
        }

        public string read(string[] commands = null)
        {
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                string ret = string.Empty;
                if (commands != null)
                {
                    foreach(string command in commands)
                    {
                        writer.Write(command);

                        // Get result from server
                        ret += reader.ReadString() + ",";
                    }
                } else
                {
                    // Get result from server
                    ret = reader.ReadString();
                }
                return ret;
            }


            
        }

        public void write(string command)
        {
            // add '\r\n'
            command += Environment.NewLine;
            // Send data to server
            using (NetworkStream stream = client.GetStream())
            using (BinaryWriter b = new BinaryWriter(stream))
            {
                b.Write(command);
                b.Flush();
            }
        }
    }
}
