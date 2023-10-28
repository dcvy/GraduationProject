using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class FeatureRepository : Repository<Feature>, IFeatureRepository
    {
        private ApplicationDbContext _db;

        public FeatureRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Feature obj)
        {
            var objFromDb = _db.Features.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;  
                }
            }
        }
    }
}
