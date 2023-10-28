using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class PartnerRepository : Repository<Partner>, IPartnerRepository
    {
        private ApplicationDbContext _db;

        public PartnerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Partner obj)
        {
            var objFromDb = _db.Partners.FirstOrDefault(u => u.Id == obj.Id);
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
