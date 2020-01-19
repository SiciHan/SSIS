using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class DisbursementItem
    {
        [Key]
        public int IdDisbursementItem { get; set;}

        public int IdDisbursement { get; set; }
        [ForeignKey("IdDisbursement")]
        public virtual Disbursement Disbursement { get; set; }

        public int IdItem { get; set; }
        [ForeignKey("IdItem")]
        public virtual Item Item { get; set;}
        public int UnitRequested { get; set; }
        public int UnitIssued { get; set;}
        public int IdStatus { get; set; }
        [ForeignKey("IdStatus")]
        public virtual Status Status { get; set; }
    }
}