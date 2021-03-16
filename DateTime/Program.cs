using System;

namespace dateTime
{
    class Program
    {
        static void Main(string[] args)
        {
            var now = DateTime.UtcNow;
            Console.WriteLine($"{now.ToString()}");
        }
    }
}
