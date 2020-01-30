using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class Employee
    {
        [Key]
        public int IdEmployee { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CodeDepartment { get; set; }

        [ForeignKey("CodeDepartment")]
        public virtual Department Department { get; set; }
        public string Title { get; set; }
        public string Tel { get; set; }

        public int IdRole { get; set; }
        [ForeignKey("IdRole")]
        public virtual Role Role { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username can only consist of alphabets")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Passwords must be between 8 and 32 in length")]
        public string HashedPassward { get; set; }

        public int RecentItem1 { get; set; }
        [ForeignKey("RecentItem1")]
        public Item Item1 { get; set; }

        public int RecentItem2 { get; set; }
        [ForeignKey("RecentItem2")]
        public Item Item2 { get; set; }
        public int RecentItem3 { get; set; }
        [ForeignKey("RecentItem3")]
        public Item Item3 { get; set; }

        //OneToMany
        //public virtual ICollection<CollectionPoint> CollectionPoints { get; set; }
        //OneToMany

        public virtual ICollection<Delegation> Delegations { get; set; }
        public virtual ICollection<CPClerk> CPClerks { get; set; }

        public virtual ICollection<Requisition> Requisitons { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public virtual ICollection<StockRecord> StockRecords { get; set; }

        [InverseProperty("From")]
        public virtual ICollection<NotificationChannel> NotificationChannelsFromMe{ get; set; }
        [InverseProperty("To")]
        public virtual ICollection<NotificationChannel> NotificationChannelsToMe { get; set; }
    }
}