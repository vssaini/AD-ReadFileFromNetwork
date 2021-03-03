using System;
using System.Diagnostics;
using System.IO;
using ReadFileFromNetwork.Helpers;

namespace ReadFileFromNetwork
{
    class Program
    {
        // 'data' is the share name and also folder on resepctive system
        const string NetworkName = @"\\192.168.1.202\data";
        const string NetworkFile = @"\\192.168.1.202\data\AboutMe.txt";

        // forward slash / used in web addresses 
        // backward slash \ used in UNC paths

        private static void Main()
        {
            try
            {
                Console.WriteLine("Reading file from network share or mapped drive...\n\n");

                ReadFromUncPath();

                // var text = File.ReadAllText(@"Z:\AboutMe.txt");
                // Console.WriteLine(text);

                //GetUncPath();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }

            // Hold till we read
            Console.ReadLine();
        }

        private static void ReadFromUncPath()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var text = Utility.ReadShareFile(NetworkName, NetworkFile);
            Console.WriteLine("Text from shared folder -\n\n{0}", text);
            Console.ResetColor();
        }

        private static void GetUncPath()
        {
            //const string path = @"Z:\25UsersImport.csv";
            const string path = @"C:\25UsersImport.csv";

            if (path.StartsWith(@"\") && Utility.PathIsNetworkPath(path))
            {
                //ReadFromUncPath();
                Console.ForegroundColor = ConsoleColor.Green;
                var uncPath = Utility.GetUNCPath(path);
                Console.WriteLine("The UNC path returned for path '{0}' -\n\n{1}", path, uncPath);
                Console.ResetColor();

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var uncPath = Utility.GetUNCPath(path);
                Console.WriteLine("The UNC path returned for path '{0}' -\n\n{1}", path, uncPath);
                Console.ResetColor();
            }

        }
    }
}
