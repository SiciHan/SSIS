namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "UserName", c => c.String(nullable: false));
            AlterColumn("dbo.Employees", "HashedPassward", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "HashedPassward", c => c.String());
            AlterColumn("dbo.Employees", "UserName", c => c.String());
        }
    }
}
