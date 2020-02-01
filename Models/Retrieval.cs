using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class Retrieval
    {
        public String Description { get; set; }
        public int IdItem { get; set; }
        public int StockUnit { get; set; }
        public String CodeDepartment { get; set; }
        public int IdRequisition { get; set; }
        public int Unit { get; set; }
        public int IdStatus { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}