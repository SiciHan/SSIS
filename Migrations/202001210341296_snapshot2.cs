namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CollectionPoints", "Time", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CollectionPoints", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
