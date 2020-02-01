using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class JoinDandDI
    {
        public Disbursement disbursement { get; set; }
        public DisbursementItem disbursementItem { get; set; }
        public Department department { get; set; }
        public Status status { get; set; }
    }
}