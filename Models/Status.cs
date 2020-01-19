using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Team8ADProjectSSIS.Models
{
    public class Status
    {
        [Key]
        public int IdStatus { get; set; }
        public string Label { get; set; }
        public virtual ICollection<DisbursementItem> DisbursementItems { get; set; }
        public virtual ICollection<Disbursement> Disbursements { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<Requisition> Requisitions { get; set; }
    }
}