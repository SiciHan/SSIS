using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class CollectionPoint
    {
        [Key]
        public int IdCollectionPt { get; set; }
        
        public string Location { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime Time { get; set; }
        public string Mapcoordinates { get; set; }

        //ManyToOne (Many CP to one Clerk)
        public int IdClerk { get; set; }

        [ForeignKey("IdClerk")]
        public virtual Employee Clerk { get; set; }

        //OneToMany
        public virtual ICollection<Department> Departments { get; set; }
    }
}