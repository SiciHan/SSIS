//@SHutong
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class Item
    {
        [Key]
        public int IdItem { get; set; }

        public int IdCategory { get; set; }
        [ForeignKey("IdCategory")]
        public Category Category { get; set; }

        public string Description { get; set; }
        public int ReorderLevel { get; set; }

        public int ReorderUnit { get; set; }
        public string unitOfMeasure { get; set; }

        public int StockUnit { get; set; }
        public int AvailableUnit { get; set; }
        public string CodeSupplier1 { get; set; }
        [ForeignKey("CodeSupplier1")]
        public Supplier Supplier1 { get; set; }
        public string CodeSupplier2 { get; set; }
        [ForeignKey("CodeSupplier2")]
        public Supplier Supplier2 { get; set; }

        public string CodeSupplier3 { get; set; }
        [ForeignKey("CodeSupplier3")]
        public Supplier Supplier3 { get; set; }
        public string Location { get; set; }

        [InverseProperty("Item1")]
        public virtual ICollection<Employee> Employees1 { get; set; }

        [InverseProperty("Item2")]
        public virtual ICollection<Employee> Employees2 { get; set; }

        [InverseProperty("Item3")]
        public virtual ICollection<Employee> Employees3 { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public virtual ICollection<DisbursementItem> DisbursementItems { get; set; }

        public virtual ICollection<SupplierItem> SupplierItems { get; set; }

        public virtual ICollection<RequisitionItem> RequisitionItems { get; set; }

        public virtual ICollection<StockRecord> StockRecords { get; set; }


    }
}