namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "IdRepresentative", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "IdRepresentative" });
            AddColumn("dbo.Departments", "Representative_IdEmployee", c => c.Int());
            CreateIndex("dbo.Departments", "Representative_IdEmployee");
            AddForeignKey("dbo.Departments", "Representative_IdEmployee", "dbo.Employees", "IdEmployee");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Departments", "Representative_IdEmployee", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "Representative_IdEmployee" });
            DropColumn("dbo.Departments", "Representative_IdEmployee");
            CreateIndex("dbo.Departments", "IdRepresentative");
            AddForeignKey("dbo.Departments", "IdRepresentative", "dbo.Employees", "IdEmployee");
        }
    }
}
