namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot10 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CPClerks", name: "IdCA", newName: "IdCPClerk");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.CPClerks", name: "IdCPClerk", newName: "IdCA");
        }
    }
}
