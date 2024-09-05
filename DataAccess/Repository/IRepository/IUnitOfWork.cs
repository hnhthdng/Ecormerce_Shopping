using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Account { get; }
        IOrderRepository Order { get; }
        ICustomerRepository Customer { get; }
        IOrderDetailRepository OrderDetail { get; }
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        ISupplierRepository Supplier { get; }
        void Save();
    }
}
