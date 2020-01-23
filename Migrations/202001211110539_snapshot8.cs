namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "CodeDepartment", c => c.String(maxLength: 128));
            CreateIndex("dbo.Employees", "CodeDepartment");
            AddForeignKey("dbo.Employees", "CodeDepartment", "dbo.Departments", "CodeDepartment");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "CodeDepartment", "dbo.Departments");
            DropIndex("dbo.Employees", new[] { "CodeDepartment" });
            AlterColumn("dbo.Employees", "CodeDepartment", c => c.String());
        }
    }
}
