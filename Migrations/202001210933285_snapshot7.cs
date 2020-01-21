namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CollectionPoints", "IdClerk", "dbo.Employees");
            DropForeignKey("dbo.Employees", "CodeDepartment", "dbo.Departments");
            DropForeignKey("dbo.Employees", "Department_CodeDepartment", "dbo.Departments");
            DropForeignKey("dbo.Departments", "Representative_IdEmployee", "dbo.Employees");
            DropIndex("dbo.Departments", new[] { "IdCollectionPt" });
            DropIndex("dbo.Departments", new[] { "Representative_IdEmployee" });
            DropIndex("dbo.CollectionPoints", new[] { "IdClerk" });
            DropIndex("dbo.Employees", new[] { "CodeDepartment" });
            DropIndex("dbo.Employees", new[] { "Department_CodeDepartment" });
            CreateTable(
                "dbo.CPClerks",
                c => new
                    {
                        IdCA = c.Int(nullable: false, identity: true),
                        IdCollectionPt = c.Int(nullable: false),
                        IdStoreClerk = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdCA)
                .ForeignKey("dbo.CollectionPoints", t => t.IdCollectionPt)
                .ForeignKey("dbo.Employees", t => t.IdStoreClerk)
                .Index(t => t.IdCollectionPt)
                .Index(t => t.IdStoreClerk);
            
            AlterColumn("dbo.Departments", "IdCollectionPt", c => c.Int());
            AlterColumn("dbo.Employees", "CodeDepartment", c => c.String());
            CreateIndex("dbo.Departments", "IdCollectionPt");
            DropColumn("dbo.Departments", "IdRepresentative");
            DropColumn("dbo.Departments", "Representative_IdEmployee");
            DropColumn("dbo.CollectionPoints", "IdClerk");
            DropColumn("dbo.Employees", "Department_CodeDepartment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Department_CodeDepartment", c => c.String(maxLength: 128));
            AddColumn("dbo.CollectionPoints", "IdClerk", c => c.Int(nullable: false));
            AddColumn("dbo.Departments", "Representative_IdEmployee", c => c.Int());
            AddColumn("dbo.Departments", "IdRepresentative", c => c.Int(nullable: false));
            DropForeignKey("dbo.CPClerks", "IdStoreClerk", "dbo.Employees");
            DropForeignKey("dbo.CPClerks", "IdCollectionPt", "dbo.CollectionPoints");
            DropIndex("dbo.CPClerks", new[] { "IdStoreClerk" });
            DropIndex("dbo.CPClerks", new[] { "IdCollectionPt" });
            DropIndex("dbo.Departments", new[] { "IdCollectionPt" });
            AlterColumn("dbo.Employees", "CodeDepartment", c => c.String(maxLength: 128));
            AlterColumn("dbo.Departments", "IdCollectionPt", c => c.Int(nullable: false));
            DropTable("dbo.CPClerks");
            CreateIndex("dbo.Employees", "Department_CodeDepartment");
            CreateIndex("dbo.Employees", "CodeDepartment");
            CreateIndex("dbo.CollectionPoints", "IdClerk");
            CreateIndex("dbo.Departments", "Representative_IdEmployee");
            CreateIndex("dbo.Departments", "IdCollectionPt");
            AddForeignKey("dbo.Departments", "Representative_IdEmployee", "dbo.Employees", "IdEmployee");
            AddForeignKey("dbo.Employees", "Department_CodeDepartment", "dbo.Departments", "CodeDepartment");
            AddForeignKey("dbo.Employees", "CodeDepartment", "dbo.Departments", "CodeDepartment");
            AddForeignKey("dbo.CollectionPoints", "IdClerk", "dbo.Employees", "IdEmployee");
        }
    }
}
