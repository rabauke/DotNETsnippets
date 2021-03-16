using System;
using System.IO;

namespace filesystem
{
    class Program
    {
        static void ListFiles(string path)
        {
            foreach (var dir in Directory.EnumerateDirectories(path)) 
            {
                ListFiles(Path.Combine(path, dir));
            }
            foreach (var file in Directory.EnumerateFiles(path)) 
            {
                var fullPath = Path.Combine(path, file);
                var creationTime = File.GetCreationTimeUtc(fullPath);
                Console.WriteLine($"{fullPath}  {creationTime}");
            }
        }


        static void Main(string[] args)
        {
            ListFiles("/usr/local/lib");
        }
    }
}
