using Kclinic.DataAccess.Repository.IRepository;
using Kclinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            Blog = new BlogRepository(_db);
            Product = new ProductRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Trial = new TrialRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);
            Launch = new LaunchRepository(_db);
            About = new AboutRepository(_db);
            Function = new FunctionRepository(_db);
            Feature = new FeatureRepository(_db);
            Partner = new PartnerRepository(_db);
            CateItem = new CateItemRepository(_db);

        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType {  get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ITrialRepository Trial { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }
		public ILaunchRepository Launch { get; private set; }
        public IFunctionRepository Function { get; private set; }
        public IFeatureRepository Feature { get; private set; }
        public IAboutRepository About { get; private set; }
        public IPartnerRepository Partner { get; private set; }
        public ICateItemRepository CateItem { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
