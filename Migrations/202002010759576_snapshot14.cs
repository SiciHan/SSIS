namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Disbursements", "IdCollectionPt", c => c.Int(nullable: true));
            AddColumn("dbo.Disbursements", "IdCollectedBy", c => c.Int(nullable: true));
            AddColumn("dbo.Disbursements", "IdDisbursedBy", c => c.Int(nullable: true));
            CreateIndex("dbo.Disbursements", "IdCollectionPt");
            CreateIndex("dbo.Disbursements", "IdCollectedBy");
            CreateIndex("dbo.Disbursements", "IdDisbursedBy");
            AddForeignKey("dbo.Disbursements", "IdCollectedBy", "dbo.Employees", "IdEmployee");
            AddForeignKey("dbo.Disbursements", "IdCollectionPt", "dbo.CollectionPoints", "IdCollectionPt");
            AddForeignKey("dbo.Disbursements", "IdDisbursedBy", "dbo.Employees", "IdEmployee");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Disbursements", "IdDisbursedBy", "dbo.Employees");
            DropForeignKey("dbo.Disbursements", "IdCollectionPt", "dbo.CollectionPoints");
            DropForeignKey("dbo.Disbursements", "IdCollectedBy", "dbo.Employees");
            DropIndex("dbo.Disbursements", new[] { "IdDisbursedBy" });
            DropIndex("dbo.Disbursements", new[] { "IdCollectedBy" });
            DropIndex("dbo.Disbursements", new[] { "IdCollectionPt" });
            DropColumn("dbo.Disbursements", "IdDisbursedBy");
            DropColumn("dbo.Disbursements", "IdCollectedBy");
            DropColumn("dbo.Disbursements", "IdCollectionPt");
        }
    }
}
