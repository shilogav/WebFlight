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
                throw new Exception("Could not connect to ip:" + ip + "port:" + port);
            }


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

        public string read(ICollection<string> commands = null)
        {
            string ret = string.Empty;
            Stream stm = client.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();
            const int sizeOfLine = 100;
            byte[] byteArray = new byte[sizeOfLine];

            if (commands != null)
            {
                foreach (string command in commands)
                {
                    byte[] ba = asen.GetBytes(command);
                    stm.Write(ba, 0, ba.Length);
                    stm.Flush();

                    int k = stm.Read(byteArray, 0, sizeOfLine);
                    string result = System.Text.Encoding.ASCII.GetString(byteArray);
                    ret += result + ',';
                }
                ret = ret.Remove(ret.Length - 1); // remove the last ','
            }
            else
            {
                int k = stm.Read(byteArray, 0, sizeOfLine);
                ret = System.Text.Encoding.ASCII.GetString(byteArray);
            }
            return ret;
        }

        public void write(string command)
        {
            // add '\r\n'
            command += Environment.NewLine;
            // Send data to server

            Stream stm = client.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(command);
            stm.Write(ba, 0, ba.Length);
            stm.Flush();
            
        }
    }
}
