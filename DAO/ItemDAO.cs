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

        public float FindPriceById(int idItem)
        {
            float price = context.SupplierItems
                .Where(x => x.IdItem == idItem)
                .Select(x => x.Price)
                .FirstOrDefault();

            return price;
        }
    }
}