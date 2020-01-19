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
            modelBuilder.Entity<Employee>()
                        .HasMany(c => c.StockRecords)
                        .WithOptional(c => c.StoreClerk)
                        .HasForeignKey(c => c.IdStoreClerk)
                        .WillCascadeOnDelete(false);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        //public DbSet Categories { get; set; }
        public DbSet CollectionPoints { get; set; }
        public DbSet Delegations { get; set; }
        public DbSet Departments { get; set; }
        public DbSet Disbursements { get; set; }
        public DbSet DisbursementItems { get; set; }
        public DbSet Employees { get; set; }
        public DbSet Items { get; set; }
        public DbSet Notifications { get; set; }
        public DbSet NotificationChannels { get; set; }
        public DbSet Operations { get; set; }
        public DbSet PurchaseOrders { get; set; }
        public DbSet PurchaseOrderDetails { get; set; }
        public DbSet Requisitions { get; set; }
        public DbSet RequisitionItems { get; set; }
        public DbSet Roles { get; set; }
        public DbSet StockRecords { get; set; }
        public DbSet Suppliers { get; set; }
        public DbSet SupplierItems { get; set; }


    }
}