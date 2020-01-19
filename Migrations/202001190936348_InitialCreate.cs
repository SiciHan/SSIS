namespace Team8ADProjectSSIS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        IdCategory = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.IdCategory);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        IdItem = c.Int(nullable: false, identity: true),
                        IdCategory = c.Int(nullable: false),
                        Description = c.String(),
                        ReorderLevel = c.Int(nullable: false),
                        ReorderUnit = c.Int(nullable: false),
                        unitOfMeasure = c.String(),
                        StockUnit = c.Int(nullable: false),
                        AvailableUnit = c.Int(nullable: false),
                        CodeSupplier1 = c.String(maxLength: 128),
                        CodeSupplier2 = c.String(maxLength: 128),
                        CodeSupplier3 = c.String(maxLength: 128),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.IdItem)
                .ForeignKey("dbo.Categories", t => t.IdCategory)
                .ForeignKey("dbo.Suppliers", t => t.CodeSupplier1)
                .ForeignKey("dbo.Suppliers", t => t.CodeSupplier2)
                .ForeignKey("dbo.Suppliers", t => t.CodeSupplier3)
                .Index(t => t.IdCategory)
                .Index(t => t.CodeSupplier1)
                .Index(t => t.CodeSupplier2)
                .Index(t => t.CodeSupplier3);
            
            CreateTable(
                "dbo.DisbursementItems",
                c => new
                    {
                        IdDisbursementItem = c.Int(nullable: false, identity: true),
                        IdDisbursement = c.Int(nullable: false),
                        IdItem = c.Int(nullable: false),
                        UnitRequested = c.Int(nullable: false),
                        UnitIssued = c.Int(nullable: false),
                        IdStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdDisbursementItem)
                .ForeignKey("dbo.Status", t => t.IdStatus)
                .ForeignKey("dbo.Disbursements", t => t.IdDisbursement)
                .ForeignKey("dbo.Items", t => t.IdItem)
                .Index(t => t.IdDisbursement)
                .Index(t => t.IdItem)
                .Index(t => t.IdStatus);
            
            CreateTable(
                "dbo.Disbursements",
                c => new
                    {
                        IdDisbursement = c.Int(nullable: false, identity: true),
                        CodeDepartment = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IdStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdDisbursement)
                .ForeignKey("dbo.Status", t => t.IdStatus)
                .ForeignKey("dbo.Departments", t => t.CodeDepartment)
                .Index(t => t.CodeDepartment)
                .Index(t => t.IdStatus);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        CodeDepartment = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        IdHead = c.Int(nullable: false),
                        IdActingHead = c.Int(nullable: false),
                        IdRepresentative = c.Int(nullable: false),
                        IdCollectionPt = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CodeDepartment)
                .ForeignKey("dbo.CollectionPoints", t => t.IdCollectionPt)
                .ForeignKey("dbo.Employees", t => t.IdActingHead)
                .ForeignKey("dbo.Employees", t => t.IdHead)
                .ForeignKey("dbo.Employees", t => t.IdRepresentative)
                .Index(t => t.IdHead)
                .Index(t => t.IdActingHead)
                .Index(t => t.IdRepresentative)
                .Index(t => t.IdCollectionPt);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        IdEmployee = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        CodeDepartment = c.String(maxLength: 128),
                        Title = c.String(),
                        Tel = c.String(),
                        IdRole = c.Int(nullable: false),
                        UserName = c.String(),
                        HashedPassward = c.String(),
                        RecentItem1 = c.Int(nullable: false),
                        RecentItem2 = c.Int(nullable: false),
                        RecentItem3 = c.Int(nullable: false),
                        Department_CodeDepartment = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IdEmployee)
                .ForeignKey("dbo.Departments", t => t.CodeDepartment)
                .ForeignKey("dbo.Roles", t => t.IdRole)
                .ForeignKey("dbo.Departments", t => t.Department_CodeDepartment)
                .ForeignKey("dbo.Items", t => t.RecentItem1)
                .ForeignKey("dbo.Items", t => t.RecentItem2)
                .ForeignKey("dbo.Items", t => t.RecentItem3)
                .Index(t => t.CodeDepartment)
                .Index(t => t.IdRole)
                .Index(t => t.RecentItem1)
                .Index(t => t.RecentItem2)
                .Index(t => t.RecentItem3)
                .Index(t => t.Department_CodeDepartment);
            
            CreateTable(
                "dbo.CollectionPoints",
                c => new
                    {
                        IdCollectionPt = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                        Time = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Mapcoordinates = c.String(),
                        IdClerk = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdCollectionPt)
                .ForeignKey("dbo.Employees", t => t.IdClerk)
                .Index(t => t.IdClerk);
            
            CreateTable(
                "dbo.Delegations",
                c => new
                    {
                        IdDelegation = c.Int(nullable: false, identity: true),
                        IdEmployee = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.IdDelegation)
                .ForeignKey("dbo.Employees", t => t.IdEmployee)
                .Index(t => t.IdEmployee);
            
            CreateTable(
                "dbo.NotificationChannels",
                c => new
                    {
                        IdNC = c.Int(nullable: false, identity: true),
                        IdFrom = c.Int(nullable: false),
                        IdTo = c.Int(nullable: false),
                        IdNotification = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.IdNC)
                .ForeignKey("dbo.Notifications", t => t.IdNotification)
                .ForeignKey("dbo.Employees", t => t.IdFrom)
                .ForeignKey("dbo.Employees", t => t.IdTo)
                .Index(t => t.IdFrom)
                .Index(t => t.IdTo)
                .Index(t => t.IdNotification);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        IdNotification = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.IdNotification);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        IdPurchaseOrder = c.Int(nullable: false, identity: true),
                        IdStoreClerk = c.Int(nullable: false),
                        IdSupplier = c.String(maxLength: 128),
                        ApprovedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OrderDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DeliverDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IdStatus = c.Int(nullable: false),
                        PurchaseRemarks = c.String(),
                    })
                .PrimaryKey(t => t.IdPurchaseOrder)
                .ForeignKey("dbo.Status", t => t.IdStatus)
                .ForeignKey("dbo.Employees", t => t.IdStoreClerk)
                .ForeignKey("dbo.Suppliers", t => t.IdSupplier)
                .Index(t => t.IdStoreClerk)
                .Index(t => t.IdSupplier)
                .Index(t => t.IdStatus);
            
            CreateTable(
                "dbo.PurchaseOrderDetails",
                c => new
                    {
                        IdPOD = c.Int(nullable: false, identity: true),
                        IdPurchaseOrder = c.Int(nullable: false),
                        IdItem = c.Int(nullable: false),
                        OrderUnit = c.Int(nullable: false),
                        DeliveredUnit = c.Int(nullable: false),
                        DeliveryRemark = c.String(),
                    })
                .PrimaryKey(t => t.IdPOD)
                .ForeignKey("dbo.Items", t => t.IdItem)
                .ForeignKey("dbo.PurchaseOrders", t => t.IdPurchaseOrder)
                .Index(t => t.IdPurchaseOrder)
                .Index(t => t.IdItem);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        IdStatus = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.IdStatus);
            
            CreateTable(
                "dbo.Requisitions",
                c => new
                    {
                        IdRequisition = c.Int(nullable: false, identity: true),
                        IdStatusCurrent = c.Int(nullable: false),
                        RaiseDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        HeadRemark = c.String(),
                        ApprovedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        WithdrawlDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Employee_IdEmployee = c.Int(),
                    })
                .PrimaryKey(t => t.IdRequisition)
                .ForeignKey("dbo.Status", t => t.IdStatusCurrent)
                .ForeignKey("dbo.Employees", t => t.Employee_IdEmployee)
                .Index(t => t.IdStatusCurrent)
                .Index(t => t.Employee_IdEmployee);
            
            CreateTable(
                "dbo.RequisitionItems",
                c => new
                    {
                        IdReqItem = c.Int(nullable: false, identity: true),
                        IdRequisiton = c.Int(nullable: false),
                        IdItem = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdReqItem)
                .ForeignKey("dbo.Items", t => t.IdItem)
                .ForeignKey("dbo.Requisitions", t => t.IdRequisiton)
                .Index(t => t.IdRequisiton)
                .Index(t => t.IdItem);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        CodeSupplier = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        ContactName = c.String(),
                        ContactTitle = c.String(),
                        Tel = c.String(),
                        SupplyDelay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CodeSupplier);
            
            CreateTable(
                "dbo.StockRecords",
                c => new
                    {
                        IdStockRecord = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IdOperation = c.Int(nullable: false),
                        IdDepartment = c.String(maxLength: 128),
                        IdSupplier = c.String(maxLength: 128),
                        IdStoreClerk = c.Int(nullable: false),
                        IdItem = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdStockRecord)
                .ForeignKey("dbo.Departments", t => t.IdDepartment)
                .ForeignKey("dbo.Items", t => t.IdItem)
                .ForeignKey("dbo.Operations", t => t.IdOperation)
                .ForeignKey("dbo.Employees", t => t.IdStoreClerk)
                .ForeignKey("dbo.Suppliers", t => t.IdSupplier)
                .Index(t => t.IdOperation)
                .Index(t => t.IdDepartment)
                .Index(t => t.IdSupplier)
                .Index(t => t.IdStoreClerk)
                .Index(t => t.IdItem);
            
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        IdOperation = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.IdOperation);
            
            CreateTable(
                "dbo.SupplierItems",
                c => new
                    {
                        IdSupplierItem = c.Int(nullable: false, identity: true),
                        IdSupplier = c.String(maxLength: 128),
                        IdItem = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.IdSupplierItem)
                .ForeignKey("dbo.Items", t => t.IdItem)
                .ForeignKey("dbo.Suppliers", t => t.IdSupplier)
                .Index(t => t.IdSupplier)
                .Index(t => t.IdItem);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        IdRole = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.IdRole);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "RecentItem3", "dbo.Items");
            DropForeignKey("dbo.Employees", "RecentItem2", "dbo.Items");
            DropForeignKey("dbo.Employees", "RecentItem1", "dbo.Items");
            DropForeignKey("dbo.DisbursementItems", "IdItem", "dbo.Items");
            DropForeignKey("dbo.DisbursementItems", "IdDisbursement", "dbo.Disbursements");
            DropForeignKey("dbo.Departments", "IdRepresentative", "dbo.Employees");
            DropForeignKey("dbo.Departments", "IdHead", "dbo.Employees");
            DropForeignKey("dbo.Employees", "Department_CodeDepartment", "dbo.Departments");
            DropForeignKey("dbo.Disbursements", "CodeDepartment", "dbo.Departments");
            DropForeignKey("dbo.Departments", "IdActingHead", "dbo.Employees");
            DropForeignKey("dbo.Employees", "IdRole", "dbo.Roles");
            DropForeignKey("dbo.Requisitions", "Employee_IdEmployee", "dbo.Employees");
            DropForeignKey("dbo.SupplierItems", "IdSupplier", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierItems", "IdItem", "dbo.Items");
            DropForeignKey("dbo.StockRecords", "IdSupplier", "dbo.Suppliers");
            DropForeignKey("dbo.StockRecords", "IdStoreClerk", "dbo.Employees");
            DropForeignKey("dbo.StockRecords", "IdOperation", "dbo.Operations");
            DropForeignKey("dbo.StockRecords", "IdItem", "dbo.Items");
            DropForeignKey("dbo.StockRecords", "IdDepartment", "dbo.Departments");
            DropForeignKey("dbo.PurchaseOrders", "IdSupplier", "dbo.Suppliers");
            DropForeignKey("dbo.Items", "CodeSupplier3", "dbo.Suppliers");
            DropForeignKey("dbo.Items", "CodeSupplier2", "dbo.Suppliers");
            DropForeignKey("dbo.Items", "CodeSupplier1", "dbo.Suppliers");
            DropForeignKey("dbo.PurchaseOrders", "IdStoreClerk", "dbo.Employees");
            DropForeignKey("dbo.Requisitions", "IdStatusCurrent", "dbo.Status");
            DropForeignKey("dbo.RequisitionItems", "IdRequisiton", "dbo.Requisitions");
            DropForeignKey("dbo.RequisitionItems", "IdItem", "dbo.Items");
            DropForeignKey("dbo.PurchaseOrders", "IdStatus", "dbo.Status");
            DropForeignKey("dbo.Disbursements", "IdStatus", "dbo.Status");
            DropForeignKey("dbo.DisbursementItems", "IdStatus", "dbo.Status");
            DropForeignKey("dbo.PurchaseOrderDetails", "IdPurchaseOrder", "dbo.PurchaseOrders");
            DropForeignKey("dbo.PurchaseOrderDetails", "IdItem", "dbo.Items");
            DropForeignKey("dbo.NotificationChannels", "IdTo", "dbo.Employees");
            DropForeignKey("dbo.NotificationChannels", "IdFrom", "dbo.Employees");
            DropForeignKey("dbo.NotificationChannels", "IdNotification", "dbo.Notifications");
            DropForeignKey("dbo.Employees", "CodeDepartment", "dbo.Departments");
            DropForeignKey("dbo.Delegations", "IdEmployee", "dbo.Employees");
            DropForeignKey("dbo.Departments", "IdCollectionPt", "dbo.CollectionPoints");
            DropForeignKey("dbo.CollectionPoints", "IdClerk", "dbo.Employees");
            DropForeignKey("dbo.Items", "IdCategory", "dbo.Categories");
            DropIndex("dbo.SupplierItems", new[] { "IdItem" });
            DropIndex("dbo.SupplierItems", new[] { "IdSupplier" });
            DropIndex("dbo.StockRecords", new[] { "IdItem" });
            DropIndex("dbo.StockRecords", new[] { "IdStoreClerk" });
            DropIndex("dbo.StockRecords", new[] { "IdSupplier" });
            DropIndex("dbo.StockRecords", new[] { "IdDepartment" });
            DropIndex("dbo.StockRecords", new[] { "IdOperation" });
            DropIndex("dbo.RequisitionItems", new[] { "IdItem" });
            DropIndex("dbo.RequisitionItems", new[] { "IdRequisiton" });
            DropIndex("dbo.Requisitions", new[] { "Employee_IdEmployee" });
            DropIndex("dbo.Requisitions", new[] { "IdStatusCurrent" });
            DropIndex("dbo.PurchaseOrderDetails", new[] { "IdItem" });
            DropIndex("dbo.PurchaseOrderDetails", new[] { "IdPurchaseOrder" });
            DropIndex("dbo.PurchaseOrders", new[] { "IdStatus" });
            DropIndex("dbo.PurchaseOrders", new[] { "IdSupplier" });
            DropIndex("dbo.PurchaseOrders", new[] { "IdStoreClerk" });
            DropIndex("dbo.NotificationChannels", new[] { "IdNotification" });
            DropIndex("dbo.NotificationChannels", new[] { "IdTo" });
            DropIndex("dbo.NotificationChannels", new[] { "IdFrom" });
            DropIndex("dbo.Delegations", new[] { "IdEmployee" });
            DropIndex("dbo.CollectionPoints", new[] { "IdClerk" });
            DropIndex("dbo.Employees", new[] { "Department_CodeDepartment" });
            DropIndex("dbo.Employees", new[] { "RecentItem3" });
            DropIndex("dbo.Employees", new[] { "RecentItem2" });
            DropIndex("dbo.Employees", new[] { "RecentItem1" });
            DropIndex("dbo.Employees", new[] { "IdRole" });
            DropIndex("dbo.Employees", new[] { "CodeDepartment" });
            DropIndex("dbo.Departments", new[] { "IdCollectionPt" });
            DropIndex("dbo.Departments", new[] { "IdRepresentative" });
            DropIndex("dbo.Departments", new[] { "IdActingHead" });
            DropIndex("dbo.Departments", new[] { "IdHead" });
            DropIndex("dbo.Disbursements", new[] { "IdStatus" });
            DropIndex("dbo.Disbursements", new[] { "CodeDepartment" });
            DropIndex("dbo.DisbursementItems", new[] { "IdStatus" });
            DropIndex("dbo.DisbursementItems", new[] { "IdItem" });
            DropIndex("dbo.DisbursementItems", new[] { "IdDisbursement" });
            DropIndex("dbo.Items", new[] { "CodeSupplier3" });
            DropIndex("dbo.Items", new[] { "CodeSupplier2" });
            DropIndex("dbo.Items", new[] { "CodeSupplier1" });
            DropIndex("dbo.Items", new[] { "IdCategory" });
            DropTable("dbo.Roles");
            DropTable("dbo.SupplierItems");
            DropTable("dbo.Operations");
            DropTable("dbo.StockRecords");
            DropTable("dbo.Suppliers");
            DropTable("dbo.RequisitionItems");
            DropTable("dbo.Requisitions");
            DropTable("dbo.Status");
            DropTable("dbo.PurchaseOrderDetails");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.Notifications");
            DropTable("dbo.NotificationChannels");
            DropTable("dbo.Delegations");
            DropTable("dbo.CollectionPoints");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
            DropTable("dbo.Disbursements");
            DropTable("dbo.DisbursementItems");
            DropTable("dbo.Items");
            DropTable("dbo.Categories");
        }
    }
}
