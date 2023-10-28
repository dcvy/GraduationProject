using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class TrialRepository : Repository<Trial>, ITrialRepository
    {
        private ApplicationDbContext _db;

        public TrialRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }



    }
}
