using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class CateItemRepository : Repository<CateItem>, ICateItemRepository
    {
        private ApplicationDbContext _db;

        public CateItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(CateItem obj)
        {
            _db.CateItems.Update(obj);
        }
    }
}
