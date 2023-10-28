using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class FunctionRepository : Repository<Function>, IFunctionRepository
    {
        private ApplicationDbContext _db;

        public FunctionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Function obj)
        {
            var objFromDb = _db.Functions.FirstOrDefault(u => u.Id == obj.Id);
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
