using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models.ViewModels
{
    public class FeatureVM
    {
        public Feature Feature { get; set; }
        public IEnumerable<Feature>? Features { get; set; }  
    }
}
