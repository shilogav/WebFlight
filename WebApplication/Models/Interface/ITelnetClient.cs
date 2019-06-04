using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulator.Model.Interface
{
    public interface ITelnetClient
    {
        void connect(string ip, int port, int maxTimeInSec = 120);
        void write(string command);
        string read(ICollection<string> commands = null);
        void disconnect();

        bool isConnected();
    }
}
