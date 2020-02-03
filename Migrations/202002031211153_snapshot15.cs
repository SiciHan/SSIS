namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot15 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "HashedPassward", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "HashedPassward", c => c.String(nullable: false, maxLength: 32));
        }
    }
}
