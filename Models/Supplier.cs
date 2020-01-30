using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class Supplier
    {
        [Key]
        public string CodeSupplier { get; set; }

        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Tel { get; set; }
        public int SupplyDelay { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string RegisNo { get; set; }

        public virtual ICollection<SupplierItem> SupplierItems { get; set; }

        public virtual ICollection<StockRecord> StockRecords{ get; set; }
        [InverseProperty("Supplier1")]
        public virtual ICollection<Item> Items1 { get; set; }
        [InverseProperty("Supplier2")]
        public virtual ICollection<Item> Items2 { get; set; }
        [InverseProperty("Supplier3")]
        public virtual ICollection<Item> Items3 { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}