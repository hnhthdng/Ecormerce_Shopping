using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _db;
        public CustomerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Customer customer)
        {
            var objFromDb = _db.Customers.FirstOrDefault(s => s.CustomerID == customer.CustomerID);
            objFromDb.Password = customer.Password;
            objFromDb.ContactName = customer.ContactName;
            objFromDb.Address = customer.Address;
            objFromDb.Phone = customer.Phone;
        }
    }
}
