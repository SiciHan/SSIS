using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class ItemDAO
    {
        private readonly SSISContext context;

        public ItemDAO()
        {
            this.context = new SSISContext();
        }

        public List<Item> GetAllItems()
        {
            List<Item> items = context.Items
                    .Include("Category")
                    .ToList();
            return items;
        }

        public List<Item> FindLowStockItems()
        {
            SSISContext context = new SSISContext();
            List<Item> items = context.Items.OfType<Item>().
                Where(x => x.AvailableUnit <= x.ReorderLevel).
                Include(i=>i.PurchaseOrderDetails).
                Include(i=>i.PurchaseOrderDetails.Select(x=>x.PurchaseOrder)).
                ToList<Item>();
            context.Dispose();
            return items;
        }
        public List<Item> FindItemsByKeyword(string searchStr)
        {
            string[] keywords = searchStr.Split(' ');
            List<Item> items = new List<Item>();
            foreach(string str in keywords)
            {
                items.AddRange(context.Items.OfType<Item>()
                .Where(x => x.Description.ToLower().Contains(str.ToLower())
                || x.Category.Label.ToLower().Contains(str.ToLower())).
                Include(i => i.PurchaseOrderDetails).
                Include(i => i.PurchaseOrderDetails.Select(x => x.PurchaseOrder))
                .ToList<Item>());
            }
            return items;
        }

        public void UpdateItem(List<int> IdDisbursementItem)
        {
            foreach (int x in IdDisbursementItem)
            {
                DisbursementItem disbursementItem = context.DisbursementItems
                                                    .Where(di => di.IdDisbursementItem == x)
                                                    .FirstOrDefault();

                Item items = context.Items.Where(i => i.IdItem == disbursementItem.IdItem).FirstOrDefault();

                if (items != null)
                {
                    if (items.StockUnit - disbursementItem.UnitIssued >= 0)
                    {
                        items.StockUnit = items.StockUnit - disbursementItem.UnitIssued;
                        context.SaveChanges();
                    }
                    if (items.AvailableUnit - disbursementItem.UnitIssued >= 0)
                    {
                        items.AvailableUnit = items.AvailableUnit - disbursementItem.UnitIssued;
                        context.SaveChanges();
                    }

                }

            }
            
        }

        public bool CheckIfLowerThanReorderLevel(int[] IdItemRetrieved) 
        {
            foreach (int IdItem in IdItemRetrieved)
            {
                Item items = context.Items.Where(i => i.IdItem == IdItem).FirstOrDefault();

                if (items != null)
                {
                    if (items.StockUnit <= items.ReorderLevel)
                        return true;
                }

            }
            return false;
        }

        public List<Item> GetDownloadableData() 
        {
            List<Item> items = context.Items.OfType<Item>().
                Include(i => i.Category).
                Include(i => i.SupplierItems).
                Include(i => i.SupplierItems.Select(x => x.Supplier)).
                Include(i => i.RequisitionItems).
                Include(i => i.RequisitionItems.Select(y => y.Requisition)).
                Include(i => i.RequisitionItems.Select(y => y.Requisition.Employee)).
                ToList<Item>();
            return items;

        }

    }
}