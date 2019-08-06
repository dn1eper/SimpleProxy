using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Proxy proxy = new Proxy(IPAddress.Parse("127.0.0.1"), 7777, "habrahabr.ru");
            proxy.Start();
        }
    }
}
