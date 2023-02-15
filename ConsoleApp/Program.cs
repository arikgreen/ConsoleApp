using System;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Add csv file as parameter");
                Console.WriteLine(">ConsoleApp.exe data.csv");
            }
            else
            {
                var reader = new DataReader();
                //reader.ImportAndPrintData("../../data.csv");
                reader.ImportAndPrintData(args[0]);
            }
        }
    }
}
