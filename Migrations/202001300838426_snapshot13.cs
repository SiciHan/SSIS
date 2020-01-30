namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot13 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Requisitions", new[] { "Employee_IdEmployee" });
            RenameColumn(table: "dbo.Requisitions", name: "Employee_IdEmployee", newName: "IdEmployee");
            AlterColumn("dbo.Requisitions", "IdEmployee", c => c.Int(nullable: false));
            CreateIndex("dbo.Requisitions", "IdEmployee");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Requisitions", new[] { "IdEmployee" });
            AlterColumn("dbo.Requisitions", "IdEmployee", c => c.Int());
            RenameColumn(table: "dbo.Requisitions", name: "IdEmployee", newName: "Employee_IdEmployee");
            CreateIndex("dbo.Requisitions", "Employee_IdEmployee");
        }
    }
}
