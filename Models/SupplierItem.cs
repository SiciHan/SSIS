using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class SupplierItem
    {
        [Key]
        public int IdSupplierItem { get; set; }
        public string IdSupplier { get; set; }
        [ForeignKey("IdSupplier")]
        public Supplier Supplier { get; set; }

        public int IdItem { get; set; }
        [ForeignKey("IdItem")]
        public Item Item { get; set; }
        public float Price { get; set; }
    }
}