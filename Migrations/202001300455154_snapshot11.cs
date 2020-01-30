namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class snapshot11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "Fax", c => c.String());
            AddColumn("dbo.Suppliers", "Address", c => c.String());
            AddColumn("dbo.Suppliers", "RegisNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "RegisNo");
            DropColumn("dbo.Suppliers", "Address");
            DropColumn("dbo.Suppliers", "Fax");
        }
    }
}
