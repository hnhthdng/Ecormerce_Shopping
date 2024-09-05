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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order order)
        {
            var objFromDb = _db.Orders.FirstOrDefault(s => s.OrderID == order.OrderID);
            objFromDb.CustomerID = order.CustomerID;
            objFromDb.AccountId = order.AccountId;
            objFromDb.OrderDate = order.OrderDate;
            objFromDb.RequiredDate = order.RequiredDate;
            objFromDb.ShippedDate = order.ShippedDate;
            objFromDb.Freight = order.Freight;
            objFromDb.ShipAddress = order.ShipAddress;
        }
    }
}
