using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace Kclinic.Models
{
    public class About
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}