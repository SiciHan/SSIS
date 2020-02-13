//@SHutong
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class Delegation
    {

        [Key]
        public int IdDelegation { get; set; }
        public int IdEmployee { get; set; }
        [ForeignKey("IdEmployee")]
        public virtual Employee Employee { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime EndDate { get; set; }
    }
}