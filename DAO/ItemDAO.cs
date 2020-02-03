using System;
using System.Collections.Generic;
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
            List<Item> items = context.Items.OfType<Item>().Where(x => x.AvailableUnit <= x.ReorderLevel).ToList<Item>();
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
                || x.Category.Label.ToLower().Contains(str.ToLower()))
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

    }
}