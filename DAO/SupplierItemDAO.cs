using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class SupplierItemDAO
    {
        public List<SupplierItem> GetSuppliersById(int itemId)
        {
            List<SupplierItem> suppliers = new List<SupplierItem>();
            using (SSISContext db = new SSISContext())
            {
                suppliers = db.SupplierItems
                    .Include("Supplier")
                    .Include("Item")
                    .Where(s => s.IdItem == itemId)
                    .ToList();
            }
            return suppliers;
        }
    }
}