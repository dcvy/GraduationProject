using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models.ViewModels
{
    public class FunctionVM
    {
        public Function Function { get; set; }
        public IEnumerable<Function>? Functions { get; set; }  
    }
}
