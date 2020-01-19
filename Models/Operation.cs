using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Team8ADProjectSSIS.Models
{
    public class Operation
    {
        [Key]
        public int IdOperation { get; set; }
        public string Label { get; set; }

        public virtual ICollection<StockRecord> StockRecords { get; set; }
    }
}