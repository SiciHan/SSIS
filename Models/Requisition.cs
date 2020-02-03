using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class Requisition
    {
        [Key]
        public int IdRequisition { get; set; }

        public int IdEmployee { get; set; }
        [ForeignKey("IdEmployee")]
        public Employee Employee { get; set; }
        public int IdStatusCurrent { get; set; }
        [ForeignKey("IdStatusCurrent")]
        public Status StatusCurrent { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime RaiseDate { get; set; }
        public string HeadRemark { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime ApprovedDate { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime WithdrawlDate { get; set; }

        

        public virtual ICollection<RequisitionItem> RequisitionItems { get; set; }
    }
}