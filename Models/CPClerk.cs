//@SHutong
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.Models
{
    public class CPClerk
    {
        [Key]
        [Column("IdCPClerk")]
        public int IdCA { get; set; }
        public int IdCollectionPt { get; set; }
        [ForeignKey("IdCollectionPt")]
        public virtual CollectionPoint CollectionPoint { get; set; }
        public int IdStoreClerk { get; set; }
        [ForeignKey("IdStoreClerk")]
        public virtual Employee StoreClerk { get; set; }
    }
}