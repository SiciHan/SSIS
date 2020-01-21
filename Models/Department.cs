using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class Department
    {
        [Key]
        public string CodeDepartment { get; set; }
        public string Name { get; set; }
        public int? IdCollectionPt { get; set; }
        [ForeignKey("IdCollectionPt")]
        public virtual CollectionPoint CollectionPt { get; set; }
        //OneToMany
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<StockRecord> StockRecords { get; set; }
        public virtual ICollection<Disbursement> Disbursements { get; set; }
    }
}