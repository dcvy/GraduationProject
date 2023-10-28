using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class LaunchRepository : Repository<Launch>, ILaunchRepository
    {
        private ApplicationDbContext _db;

        public LaunchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Launch obj)
        {
            var objFromDb = _db.Launchs.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;  
                }
            }
        }
    }
}
