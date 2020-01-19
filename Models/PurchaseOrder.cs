using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int IdPurchaseOrder { get; set; }
        public int IdStoreClerk { get; set; }
        [ForeignKey("IdStoreClerk")]
        public Employee StoreClerk { get; set; }

        public string IdSupplier { get; set; }//supplier's pk is string
        [ForeignKey("IdSupplier")]
        public Supplier Supplier { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime ApprovedDate { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime DeliverDate { get; set; }

        public int IdStatus { get; set; }//supplier's pk is string
        [ForeignKey("IdStatus")]
        public Status Status { get; set; }
        public string PurchaseRemarks { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}