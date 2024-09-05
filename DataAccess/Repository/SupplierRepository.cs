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
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private readonly ApplicationDbContext _db;

        public SupplierRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Supplier supplier)
        {
            var objFromDb = _db.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID);
            if (objFromDb != null)
            {
                objFromDb.CompanyName = supplier.CompanyName;
                objFromDb.Address = supplier.Address;
                objFromDb.Phone = supplier.Phone;
            }
        }
    }
}
