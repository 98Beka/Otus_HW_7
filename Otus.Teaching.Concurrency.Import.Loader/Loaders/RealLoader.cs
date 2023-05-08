using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Loader.Loaders {
    internal class RealLoader : IDataLoader {
        private readonly IDataParser<List<Customer>> _dataParser;
        private readonly ICustomerRepository _customerRepository;
        private int _numOfTryingLimit;
        public RealLoader(IDataParser<List<Customer>> dataParser, ICustomerRepository customerRepository, int numOfTryingLimit) {
            _dataParser = dataParser;
            _customerRepository = customerRepository;
            _numOfTryingLimit = numOfTryingLimit;
        }
        public void LoadData() {
            List<Customer> customers = null;
            int tryNum = 0;
            while (customers == null && tryNum < _numOfTryingLimit) {
                customers = _dataParser.Parse();
                tryNum++; 
            }

            Console.WriteLine($"tryings count == {tryNum}");
            if(customers == null) {
                Console.WriteLine("costomers == null");
                return;
            }
            foreach (var customer in customers)
                _customerRepository.AddCustomer(customer);
        }
    }
}
