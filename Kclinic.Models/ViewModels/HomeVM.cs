using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models.ViewModels
{
	public class HomeVM
	{
		public IEnumerable<Kclinic.Models.Blog> Blogs { get; set; }
		public IEnumerable<Kclinic.Models.Product> Products { get; set; }
		public IEnumerable<Kclinic.Models.Launch> Launchs { get; set; }
        public IEnumerable<Kclinic.Models.About> Abouts { get; set; }
        public IEnumerable<Kclinic.Models.Function> Functions { get; set; }
        public IEnumerable<Kclinic.Models.Feature> Features { get; set; }
        public IEnumerable<Kclinic.Models.Partner> Partners { get; set; }
        public IEnumerable<Kclinic.Models.ShoppingCart> ShoppingCarts { get; set; }
        public IEnumerable<Kclinic.Models.CateItem> CateItems { get; set; }
        public Trial Trial { get; set; }
        public string Result { get; set; }
    }
}
