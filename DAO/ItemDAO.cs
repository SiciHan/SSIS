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

        public bool IsStockLow()
        {    
            Item item = context.Items.OfType<Item>().Where(x => x.AvailableUnit <= x.ReorderLevel).FirstOrDefault();
            return item != null;
        }
    }
}