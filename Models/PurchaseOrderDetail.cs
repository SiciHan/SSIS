using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class PurchaseOrderDetail
    {
        [Key]
        public int IdPOD { get; set; }

        public int IdPurchaseOrder { get; set; }
        [ForeignKey("IdPurchaseOrder")]
        public PurchaseOrder PurchaseOrder { get; set; }

        public int IdItem { get; set; }
        [ForeignKey("IdItem")]
        public Item Item { get; set; }

        public int OrderUnit { get; set; }
        public int DeliveredUnit { get; set; }
        public string DeliveryRemark { get; set; }
    }
}