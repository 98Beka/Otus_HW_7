using System;
using System.Collections.Generic;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class CustomerRepository
        : ICustomerRepository
    {
        private List<Customer> _customerList = new List<Customer>();
        public void AddCustomer(Customer customer)
        {
            _customerList.Add(customer);
        }
    }
}