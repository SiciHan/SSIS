using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class StockRecordDAO
    {
        private readonly SSISContext context;

        public StockRecordDAO()
        {
            this.context = new SSISContext();
        }

        public List<StockRecord> FindVoucher()
        {
            List<StockRecord> vouchers = context.StockRecords
                .Include("Operation")
                .Include("Department")
                .Include("Supplier")
                .Include("StoreClerk")
                .Include("Item")
                .Where(x => x.IdOperation == 3 || x.IdOperation == 4 || x.IdOperation == 5 || x.IdOperation == 6)
                .ToList();

            return vouchers;
        }

        public void UpdateVoucherToApproved(List<StockRecord> vouchers)
        {
            foreach(StockRecord voucher in vouchers)
            {
                StockRecord temp = context.StockRecords
                    .Where(x => x.IdStockRecord == voucher.IdStockRecord)
                    .FirstOrDefault();
                if(temp.IdOperation == 3)
                {
                    temp.IdOperation = 7;
                }
                else if(temp.IdOperation == 4)
                {
                    temp.IdOperation = 9;
                }
                else if (temp.IdOperation == 5)
                {
                    temp.IdOperation = 12;
                }
                else if (temp.IdOperation == 6)
                {
                    temp.IdOperation = 14;
                }
                context.SaveChanges();
            }
        }

        public void UpdateVoucherToRejected(List<StockRecord> vouchers)
        {
            foreach (StockRecord voucher in vouchers)
            {
                StockRecord temp = context.StockRecords
                    .Where(x => x.IdStockRecord == voucher.IdStockRecord)
                    .FirstOrDefault();
                if (temp.IdOperation == 3)
                {
                    temp.IdOperation = 8;
                }
                else if (temp.IdOperation == 4)
                {
                    temp.IdOperation = 10;
                }
                else if (temp.IdOperation == 5)
                {
                    temp.IdOperation = 11;
                }
                else if (temp.IdOperation == 6)
                {
                    temp.IdOperation = 13;
                }
                context.SaveChanges();
            }
        }
    }
}