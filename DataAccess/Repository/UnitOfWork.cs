using DataAccess.Data;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Account = new AccountRepository(_db);
            Order = new OrderRepository(_db);
            Customer = new CustomerRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Product = new ProductRepository(_db);
            Category = new CategoryRepository(_db);
            Supplier = new SupplierRepository(_db); 

        }
        public IAccountRepository Account { get; private set; }
        public IOrderRepository Order { get; private set; }
        public ICustomerRepository Customer { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public ISupplierRepository Supplier { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
