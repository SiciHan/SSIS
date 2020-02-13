//@SHutong
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class StockRecord
    {
        [Key]
        public int IdStockRecord { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Date { get; set; }
        public int IdOperation { get; set; }
        [ForeignKey("IdOperation")]
        public Operation Operation { get; set; }
        public string IdDepartment { get; set; }
        [ForeignKey("IdDepartment")]
        public Department Department { get; set; }

        public string IdSupplier { get; set; }
        [ForeignKey("IdSupplier")]
        public Supplier Supplier { get; set; }

        public int? IdStoreClerk { get; set; }
        [ForeignKey("IdStoreClerk")]
        public Employee StoreClerk { get; set; }

        public int IdItem { get; set; }
        [ForeignKey("IdItem")]
        public Item Item { get; set; }
        public int Unit { get; set; }
    }
}