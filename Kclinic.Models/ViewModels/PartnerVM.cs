using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models.ViewModels
{
    public class PartnerVM
    {
        public Partner Partner { get; set; }
        public IEnumerable<Partner>? Partners { get; set; }  
    }
}
