using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.DataAccess.Parsers;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;
using Otus.Teaching.Concurrency.Import.Loader.Loaders;

namespace Otus.Teaching.Concurrency.Import.Loader
{
    class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.xml");
        private const string _fileGenAppPath = "D:/OTUS/Otus_HW_7/Otus.Teaching.Concurrency.Import.DataGenerator.App/bin/Release/netcoreapp3.1/";
        private const string _fileGenAppName = "Otus.Teaching.Concurrency.Import.DataGenerator.App.exe";
        private const int _numOfTrying = 100000;
        private const int _numOfCustomers = 1000000;
        private const int _numOfThreads = 1;
        private const bool _isExternalForGenFile = false; //if true than will be started as process

        static void Main(string[] args)
        {
            if(args.Length == 0)
                args = GenArgs();

            if (args != null && args.Length == 1)
                _dataFilePath = args[0];

            GenerateXmlFileOfCustomers(args);

            var xmlParser = new XmlParser(_dataFilePath, _numOfThreads);
            var castomerRepository = new CustomerRepository();
            var loader = new RealLoader(xmlParser, castomerRepository, _numOfTrying);

            var timer = new Stopwatch();
            timer.Start();
            loader.LoadData();
            timer.Stop();

            Console.WriteLine($"it took {timer.ElapsedMilliseconds} ms to load {_numOfCustomers} customers from xml to database");
            Console.ReadLine();
        }

        private static void GenerateXmlFileOfCustomers(string[] args) {
            Console.WriteLine($"Loader started with process Id {Process.GetCurrentProcess().Id}...");
            if (args.Length >= 3 && args[2] == true.ToString()) {
                var path = Path.Combine(_fileGenAppPath, _fileGenAppName);
                var processInfo = new ProcessStartInfo(path);
                processInfo.Arguments = ConvertStrArrToStr(args);
                var process =  Process.Start(processInfo);
                process.WaitForExit();
            } 
            else 
                GenerateCustomersDataFile();
        }

        private static void GenerateCustomersDataFile()
        {
            var xmlGenerator = new XmlGenerator(_dataFilePath, 1000);
            xmlGenerator.Generate();
        }
        private static string[] GenArgs() {
            return new[] {
                _dataFilePath,
                _numOfCustomers.ToString(),
                _isExternalForGenFile.ToString()
            };
        }

        private static string ConvertStrArrToStr(string[] args) {
            var strBuilder = new StringBuilder();

            foreach (var arg in args)
                strBuilder.Append(" " + arg);
            return strBuilder.ToString();
        }
    }
}