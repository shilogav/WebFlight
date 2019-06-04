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
    public class MyTcpServer : ITelnetServer
    {
        private TcpListener listener;
        private bool isOpen = false;
        private volatile bool stop = false;

        // avoid from closing the server while reading.
        private Object numOfReadingLocker = new Object();
        private Mutex emptyReadingMutex = new Mutex();
        private volatile int numOfReadingThreads = 0;

        public MyTcpServer() 
        {
        }

        ~MyTcpServer()
        {
            emptyReadingMutex.Dispose();
        }

        public void Start(string ip, int port)
        {
            stop = false;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            listener = new TcpListener(ep);
            listener.Start();
            isOpen = true;
            Console.WriteLine("Waiting for connections...");
            ListenToCilents();
        }

        private void updateValues(string[] values, IModel model)
        {
            try
            {
                model.Longitude = Convert.ToDouble(values[0]);
                model.Latitude = Convert.ToDouble(values[1]);
            }
            catch (Exception e) { }
        }

        void readFromClient(TcpClient client)
        {
            IModel model = MyModel.Instance;
            BinaryReader reader = new BinaryReader(client.GetStream());
            while (!stop)
            {
                
                string str = "";

                // avoid from closing the server while someone reading.
                /*lock (numOfReadingLocker) {
                    if (numOfReadingThreads == 0)
                    {
                        emptyReadingMutex.WaitOne();
                    }
                    numOfReadingThreads++;
                }*/

                try {
                    str = reader.ReadString();
                } catch {
                    str = "";
                }
                /*lock (numOfReadingLocker) {
                    numOfReadingThreads--;
                    if (numOfReadingThreads == 0)
                    {
                        emptyReadingMutex.ReleaseMutex();
                    }
                }*/

                string[] vals = str.Split(
                            new[] { ',' },
                                StringSplitOptions.None);
                updateValues(vals, model);
                //Thread.Sleep(200);

                

            }
        }

        private void ListenToCilents()
        {
            Thread thread = new Thread(() =>
            {
                TcpClient client = listener.AcceptTcpClient();
                readFromClient(client);
            });
            thread.Start();
        }
        public void Stop()
        {
            if (listener != null)
            {
                isOpen = false;
                stop = true;

                lock (numOfReadingLocker)
                {
                    emptyReadingMutex.WaitOne();
                }
                listener.Stop();
                emptyReadingMutex.ReleaseMutex();
            }
        }

        public bool IsOpen()
        {
            return listener != null && isOpen == true;
        }
    }
}
