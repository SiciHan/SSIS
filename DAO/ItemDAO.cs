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
        public static List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            using(SSISContext db = new SSISContext())
            {
                items = db.Items
                    .Include("Category")
                    .ToList();
            }
            return items;
        }
    }
}