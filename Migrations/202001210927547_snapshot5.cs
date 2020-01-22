namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "IdActingHead", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "IdActingHead" });
            DropColumn("dbo.Departments", "IdActingHead");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "IdActingHead", c => c.Int(nullable: false));
            CreateIndex("dbo.Departments", "IdActingHead");
            AddForeignKey("dbo.Departments", "IdActingHead", "dbo.Employees", "IdEmployee");
        }
    }
}
