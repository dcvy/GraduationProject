using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class AboutRepository : Repository<About>, IAboutRepository
    {
        private ApplicationDbContext _db;

        public AboutRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(About obj)
        {
            var objFromDb = _db.Abouts.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Content = obj.Content;
				objFromDb.Link = obj.Link;
				if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}