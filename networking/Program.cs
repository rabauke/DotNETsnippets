using System;
using System.Net;
using System.Net.Sockets;

namespace networking
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                IPHostEntry hostInfo = Dns.GetHostEntry("www.contoso.c");
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.SocketErrorCode);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
