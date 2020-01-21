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

       /* public int IdHead { get; set; }
        [ForeignKey("IdHead")]
        public virtual Employee Head { get; set; }*/

        /*public int IdActingHead { get; set; }
        [ForeignKey("IdActingHead")]
        public virtual Employee ActingHead { get; set; }*/

        //public int IdRepresentative { get; set; }
        //[ForeignKey("IdRepresentative")]
        //public virtual Employee Representative { get; set; }

        public int? IdCollectionPt { get; set; }
        [ForeignKey("IdCollectionPt")]
        public virtual CollectionPoint CollectionPt { get; set; }
        //OneToMany
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<StockRecord> StockRecords { get; set; }
        public virtual ICollection<Disbursement> Disbursements { get; set; }
    }
}