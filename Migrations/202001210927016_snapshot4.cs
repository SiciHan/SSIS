namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "IdHead", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "IdHead" });
            DropColumn("dbo.Departments", "IdHead");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "IdHead", c => c.Int(nullable: false));
            CreateIndex("dbo.Departments", "IdHead");
            AddForeignKey("dbo.Departments", "IdHead", "dbo.Employees", "IdEmployee");
        }
    }
}
