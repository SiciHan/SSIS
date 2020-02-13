//@SHutong
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
        public CollectionPoint()
        {
        }

        public CollectionPoint(CollectionPoint cp)
        {
            this.Location = cp.Location;
            this.Time = cp.Time;
            this.Mapcoordinates = cp.Mapcoordinates;
        }

      //  public CollectionPoint() { }

        [Key]
        public int IdCollectionPt { get; set; }
        
        public string Location { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan Time { get; set; }
        public string Mapcoordinates { get; set; }

        //ManyToOne (Many CP to one Clerk)
        /*public int IdClerk { get; set; }*/

        /*[ForeignKey("IdClerk")]
        public virtual Employee Clerk { get; set; }*/

        //OneToMany
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<CPClerk> CPClerks { get; set; }
        public virtual ICollection<Disbursement> Disbursements { get; set; }
    }
}