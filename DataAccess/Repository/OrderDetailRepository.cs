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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderDetail orderDetail)
        {
            var objFromDb = dbSet.FirstOrDefault(s => s.OrderID == orderDetail.OrderID && s.ProductID == orderDetail.ProductID);
            objFromDb.UnitPrice = orderDetail.UnitPrice;
            objFromDb.Quantity = orderDetail.Quantity;
        }
    }
}
