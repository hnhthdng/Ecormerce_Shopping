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
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(s => s.ProductID == product.ProductID);
            objFromDb.ProductName = product.ProductName;
            objFromDb.SupplierID = product.SupplierID;
            objFromDb.CategoryID = product.CategoryID;
            objFromDb.QuantityPerUnit = product.QuantityPerUnit;
            objFromDb.UnitPrice = product.UnitPrice;
            if (product.ProductImageURL != null)
            {
                objFromDb.ProductImageURL = product.ProductImageURL;
            }
        }
    }
}
