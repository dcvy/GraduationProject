using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category {  get; }
        ICoverTypeRepository CoverType {  get; }
        IBlogRepository Blog { get; }
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ITrialRepository Trial { get; }
		IOrderDetailRepository OrderDetail { get; }
		IOrderHeaderRepository OrderHeader { get; }
        ILaunchRepository Launch { get; }
        IAboutRepository About { get; }
        IFunctionRepository Function { get; }
        IFeatureRepository Feature { get; }
        IPartnerRepository Partner { get; }
        ICateItemRepository CateItem { get; }
        void Save();
    }
}
