namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StockRecords", new[] { "IdStoreClerk" });
            AlterColumn("dbo.StockRecords", "IdStoreClerk", c => c.Int(nullable: true));
            CreateIndex("dbo.StockRecords", "IdStoreClerk");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockRecords", new[] { "IdStoreClerk" });
            AlterColumn("dbo.StockRecords", "IdStoreClerk", c => c.Int(nullable: false));
            CreateIndex("dbo.StockRecords", "IdStoreClerk");
        }
    }
}
