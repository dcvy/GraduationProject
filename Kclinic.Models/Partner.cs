using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models
{
    public class Partner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

    }
}
