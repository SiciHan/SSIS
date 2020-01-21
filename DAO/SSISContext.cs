using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class SSISContext:DbContext

    {
        public SSISContext() : base("Database=SSIS;Integrated Security=True")
        {
            Database.SetInitializer(new SSISInitializer<SSISContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Status>().ToTable("Status");

            //the stockRecords can have optional store clerk 
            modelBuilder.Entity<Employee>()
                        .HasMany(c => c.StockRecords)
                        .WithOptional(c => c.StoreClerk)
                        .HasForeignKey(c => c.IdStoreClerk)
                        .WillCascadeOnDelete(false);
            // the department can have optional CP (e.g. store department no need a CP)

          modelBuilder.Entity<CollectionPoint>()
                .HasMany(c => c.Departments)
                .WithOptional(d=>d.CollectionPt)
                .HasForeignKey(k=>k.IdCollectionPt)
                .WillCascadeOnDelete(false);
           /* modelBuilder.Entity<Employee>()
             .HasRequired<Department>(c => c.Department)
             .WithMany(d=>d.Employees)
             .HasForeignKey(s=>s.CodeDepartment)
             .WillCascadeOnDelete();*/
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        //public DbSet Categories { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<Delegation> Delegations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<DisbursementItem> DisbursementItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationChannel> NotificationChannels { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionItem> RequisitionItems { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StockRecord> StockRecords { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierItem> SupplierItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CPClerk> CPClerks { get; set; }
    }
}