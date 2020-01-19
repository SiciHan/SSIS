using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class RequisitionItem
    {
        [Key]
        public int IdReqItem { get; set; }
        public int IdRequisiton { get; set; }
        [ForeignKey("IdRequisiton")]
        public Requisition Requisition { get; set; }
        public int IdItem { get; set; }
        [ForeignKey("IdItem")]
        public Item Item { get; set; }
        public int Unit { get; set; }
    }
}