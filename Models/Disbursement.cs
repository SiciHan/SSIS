//@SHutong
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class Disbursement
    {
        [Key]
        public int IdDisbursement { get; set; }

        public string CodeDepartment { get; set; }
        [ForeignKey("CodeDepartment")]
        public virtual Department Department { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Date { get; set; }

        public int IdStatus { get; set; }
        [ForeignKey("IdStatus")]
        public virtual Status Status { get; set; }

        public int? IdCollectionPt { get; set; }
        [ForeignKey("IdCollectionPt")]
        public virtual CollectionPoint CollectionPoint { get; set; }

        public int? IdCollectedBy { get; set; }
        [ForeignKey("IdCollectedBy")]
        public virtual Employee CollectedBy { get; set; }

        public int? IdDisbursedBy { get; set; }
        [ForeignKey("IdDisbursedBy")]
        public virtual Employee DisbursedBy { get; set; }

        public virtual ICollection<DisbursementItem> DisbursementItems{ get; set; }
    }
}