
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductManagement.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
      
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Smallimage { get; set; }
        public string Largeimage { get; set; }

        [NotMapped]
        public HttpPostedFileBase SImage { get; set; }
        [NotMapped]
        public HttpPostedFileBase LImage { get; set; }


        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}