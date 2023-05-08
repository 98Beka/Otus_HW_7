using System.Collections.Generic;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using System.Xml.Serialization;
using System;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers {
    public class XmlParser
        : IDataParser<List<Customer>>
    {
        private readonly string _path;
        private readonly int _numOfThreads;
        public XmlParser(string path, int numOfThreads) {
            _path = path;
            _numOfThreads = numOfThreads;
        }


        public List<Customer> Parse()
        {
            try {


                //var serializer = new XmlSerializer(typeof(List<Customer>));
                //using (var reader = XmlReader.Create(_path))
                //    return (List<Customer>)serializer.Deserialize(reader);

                var serializer = new XmlSerializer(typeof(List<Customer>));
                List<Customer> customers = new List<Customer>();

                using (var reader = XmlReader.Create(_path)) {
                    var threads = new List<Thread>();
                    int count = 0;
                    int totalDepth = reader.Depth;
                    for (int i = 0; i < _numOfThreads; i++) {

                        var thread = new Thread(() =>
                        {
                            while (reader.Read() && count < 0.5 * totalDepth) {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Customer") {
                                    if (reader.Depth >= 0.5 * totalDepth) {
                                        customers.Add((Customer)serializer.Deserialize(reader.ReadSubtree()));
                                    } else {
                                        reader.Skip();
                                    }
                                    count++;
                                }
                            }
                        });

                        threads.Add(thread);
                    }

                    foreach(var thread in threads)
                        thread.Start();

                    foreach (var thread in threads)
                        thread.Join();
                }
                return customers;

            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return default;
            }
        }
    }
}