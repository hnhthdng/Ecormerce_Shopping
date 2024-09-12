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
    public class AdsRepository : Repository<Ads>, IAdsRepository
    {
        private readonly ApplicationDbContext _db;

        public AdsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Ads ads)
        {
            var objFromDb = _db.Adses.FirstOrDefault(s => s.Id == ads.Id);
            objFromDb.Title = ads.Title;
            objFromDb.Content = ads.Content;
            objFromDb.StartDate = ads.StartDate;
            objFromDb.EndDate = ads.EndDate;
        }
    }
}
